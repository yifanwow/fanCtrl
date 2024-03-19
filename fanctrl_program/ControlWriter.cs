using System.IO;
using System.Text;
using System.Windows.Forms;

namespace fanCtrl
{
    public class ControlWriter : TextWriter
    {
        private RichTextBox _output; // RichTextBox 控件

        public ControlWriter(RichTextBox output)
        {
            _output = output;
        }

        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }

        public override void Write(char value)
        {
            MethodInvoker append = delegate
            {
                _output.AppendText(value.ToString());
            };
            _output.BeginInvoke(append);
            _output.ScrollToCaret();
        }
    }
}
