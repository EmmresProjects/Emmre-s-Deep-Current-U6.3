using System;

namespace DeepCurrent
{
    // MVP Pattern: owns the selected loadout and responds to customization actions.
    public sealed class CustomizePresenter
    {
        readonly ICustomizeView view;
        readonly GameRoot game;
        readonly Action returnToTitle;
        int selectedSkin;
        int selectedPaint;

        public CustomizePresenter(ICustomizeView customizeView, GameRoot gameRoot, Action backToTitle)
        {
            view = customizeView;
            game = gameRoot;
            returnToTitle = backToTitle;
            ConfigureOptions();

            view.SkinSelected += SelectSkin;
            view.PaintSelected += SelectPaint;
            view.ApplyRequested += ApplyLoadout;
            view.BackRequested += ReturnToTitle;
        }

        void ConfigureOptions()
        {
            var catalog = game.diverCatalog;
            if (catalog == null)
            {
                view.SetOptions(new string[0], new UnityEngine.Sprite[0], new UnityEngine.Color[0]);
                return;
            }

            var skins = catalog.skins ?? new DiverCatalog.SkinOption[0];
            var paints = catalog.paints ?? new DiverCatalog.PaintOption[0];
            var names = new string[skins.Length];
            var sprites = new UnityEngine.Sprite[skins.Length];
            var colors = new UnityEngine.Color[paints.Length];
            for (int i = 0; i < skins.Length; i++)
            {
                names[i] = skins[i].displayName;
                sprites[i] = skins[i].sprite;
            }
            for (int i = 0; i < paints.Length; i++)
                colors[i] = paints[i].tint;
            view.SetOptions(names, sprites, colors);
        }

        public void Show()
        {
            var catalog = game.diverCatalog;
            selectedSkin = catalog != null ? catalog.ClampSkinIndex(DiverLoadout.SkinIndex) : 0;
            selectedPaint = catalog != null ? catalog.ClampPaintIndex(DiverLoadout.PaintIndex) : 0;
            view.SetSelection(selectedSkin, selectedPaint);
            view.Show();
        }

        public void Hide() => view.Hide();

        void SelectSkin(int index)
        {
            var catalog = game.diverCatalog;
            selectedSkin = catalog != null ? catalog.ClampSkinIndex(index) : 0;
            view.SetSelection(selectedSkin, selectedPaint);
        }

        void SelectPaint(int index)
        {
            var catalog = game.diverCatalog;
            selectedPaint = catalog != null ? catalog.ClampPaintIndex(index) : 0;
            view.SetSelection(selectedSkin, selectedPaint);
        }

        void ApplyLoadout()
        {
            DiverLoadout.Select(selectedSkin, selectedPaint);
            DiverLoadout.Save();
            DiverBuilder.FromLoadout(game.diverCatalog).ApplyTo(game.Player);
            returnToTitle();
        }

        void ReturnToTitle() => returnToTitle();
    }
}
