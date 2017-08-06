using CommonStyleLib.Models;
using System;
using static CommonStyleLib.ExMessageBox.ExMessageBoxBase;

namespace CommonStyleLib.ExMessageBox.Models
{
    public class ExMassageBoxModel : ModelBase
    {
        public ExMassageBoxModel(string text, string title, MessageType mType)
        {
            this.text = text;
            this.title = title;
            this.mType = mType;
        }

        private string text = string.Empty;
        public string Text
        {
            get => text;
            set => SetProperty(ref text, value);
        }
        private string title = string.Empty;
        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }
        private MessageType mType;

        private DialogResult dr;
        public DialogResult Result
        {
            get => dr;
            set => dr = value;
        }

        public void Load(IntPtr handle)
        {
            Text = text;
            Title = title;

            FlashWind(handle);

            switch(mType)
            {
                case MessageType.Asterisk:
                    System.Media.SystemSounds.Asterisk.Play();
                    break;
                case MessageType.Beep:
                    System.Media.SystemSounds.Beep.Play();
                    break;
                case MessageType.Exclamation:
                    System.Media.SystemSounds.Exclamation.Play();
                    break;
                case MessageType.Hand:
                    System.Media.SystemSounds.Hand.Play();
                    break;
                case MessageType.Question:
                    System.Media.SystemSounds.Question.Play();
                    break;
            }
        }
    }
}
