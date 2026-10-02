using UnityEngine;
using UnityEngine.UI;

namespace HoldfastAR.UI
{
    public class PlacementPanel : UIPanel
    {
        private Text _instruction;
        private Text _status;
        private float _pulse;

        protected override void OnBuild()
        {
            RectTransform banner = UIFactory.Image(Root, "Banner", UITheme.Panel, UISkin.Panel).rectTransform
                .Place(new Vector2(0.5f, 1f), new Vector2(960, 300), new Vector2(0, -80));
            _instruction = UIFactory.Text(banner, "Instruction", "", UITheme.BodySize + 4, UITheme.Text, style: FontStyle.Bold);
            _instruction.rectTransform.Stretch(30);
            _instruction.rectTransform.offsetMin = new Vector2(30, 90);

            _status = UIFactory.Text(banner, "Status", "", UITheme.SmallSize, UITheme.Accent);
            _status.rectTransform.Place(new Vector2(0.5f, 0f), new Vector2(900, 80), new Vector2(0, 20));

            UIFactory.Button(Root, "BACK", UITheme.ButtonNeutral, Game.ReturnToMainMenu).GetComponent<RectTransform>()
                .Place(new Vector2(0.5f, 0f), new Vector2(420, 130), new Vector2(0, 90));
        }

        protected override void OnShow() => SetPlaneCount(0);

        public void SetPlaneCount(int count)
        {
            if (count == 0)
            {
                _instruction.text = "Scan the ground slowly to find a landing zone";
                _status.text = "Searching for flat ground...";
            }
            else
            {
                _instruction.text = "Tap your name tile to drop the Holdfast dome";
                _status.text = count == 1 ? "1 surface detected" : $"{count} surfaces detected";
            }
        }

        public override void Tick(float deltaTime)
        {
            _pulse += deltaTime * 3f;
            _status.color = Color.Lerp(UITheme.Accent, UITheme.Text, (Mathf.Sin(_pulse) + 1f) * 0.25f);
        }
    }
}
