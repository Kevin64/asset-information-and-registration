using ConstantsDLL;
using System.Drawing;
using System.Windows.Forms;

namespace HardwareInformation
{
    public class CustomColors
    {
        public static void groupBox_PaintDarkTheme(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            DrawGroupBox(box, e.Graphics, StringsAndConstants.DARK_FORECOLOR, StringsAndConstants.DARK_SUBTLE_LIGHTLIGHTCOLOR);
        }

        public static void groupBox_PaintLightTheme(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            DrawGroupBox(box, e.Graphics, StringsAndConstants.LIGHT_FORECOLOR, StringsAndConstants.LIGHT_SUBTLE_DARKDARKCOLOR);
        }

        public static void DrawGroupBox(GroupBox box, Graphics g, Color textColor, Color borderColor)
        {
            if (box != null)
            {
                Brush textBrush = new SolidBrush(textColor);
                Brush borderBrush = new SolidBrush(borderColor);
                Pen borderPen = new Pen(borderBrush);
                SizeF strSize = g.MeasureString(box.Text, box.Font);
                Rectangle rect = new Rectangle(box.ClientRectangle.X,
                                               box.ClientRectangle.Y + (int)(strSize.Height / 2),
                                               box.ClientRectangle.Width - 1,
                                               box.ClientRectangle.Height - (int)(strSize.Height / 2) - 1);

                // Clear text and border
                g.Clear(box.BackColor);

                // Draw text
                g.DrawString(box.Text, box.Font, textBrush, box.Padding.Left, 0);

                // Drawing Border
                //Left
                g.DrawLine(borderPen, rect.Location, new Point(rect.X, rect.Y + rect.Height));
                //Right
                g.DrawLine(borderPen, new Point(rect.X + rect.Width, rect.Y), new Point(rect.X + rect.Width, rect.Y + rect.Height));
                //Bottom
                g.DrawLine(borderPen, new Point(rect.X, rect.Y + rect.Height), new Point(rect.X + rect.Width, rect.Y + rect.Height));
                //Top1
                g.DrawLine(borderPen, new Point(rect.X, rect.Y), new Point(rect.X + box.Padding.Left, rect.Y));
                //Top2
                g.DrawLine(borderPen, new Point(rect.X + box.Padding.Left + (int)(strSize.Width), rect.Y), new Point(rect.X + rect.Width, rect.Y));
            }
        }
    }

    public class ModifiedToolStrip : ToolStripRenderer
    {
        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            e.ArrowColor = Color.White;
            base.OnRenderArrow(e);
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            e.Graphics.Clear(Color.Red);
            base.OnRenderMenuItemBackground(e);
        }



    }

    public class ModifiedToolStripProfessionalLightTheme : ToolStripProfessionalRenderer
    {
        public ModifiedToolStripProfessionalLightTheme() : base(new ModifiedColorsLightTheme()) { }

        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            e.ArrowColor = Color.Black;
            base.OnRenderArrow(e);
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            if (e.ToolStrip is StatusStrip)
                e.Graphics.DrawLine(Pens.Gray, 0, 0, e.ToolStrip.Width, 0);
            else
                base.OnRenderToolStripBorder(e);
        }

        protected override void OnRenderLabelBackground(ToolStripItemRenderEventArgs e)
        {
            using (var brush = new SolidBrush(e.Item.BackColor))
            {
                e.Graphics.FillRectangle(brush, new Rectangle(Point.Empty, e.Item.Size));
            }
        }
    }

    public class ModifiedToolStripProfessionalDarkTheme : ToolStripProfessionalRenderer
    {
        public ModifiedToolStripProfessionalDarkTheme() : base(new ModifiedColorsDarkTheme()) { }

        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            e.ArrowColor = Color.White;
            base.OnRenderArrow(e);
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            if (e.ToolStrip is StatusStrip)
                e.Graphics.DrawLine(Pens.Gray, 0, 0, e.ToolStrip.Width, 0);
            else
                base.OnRenderToolStripBorder(e);
        }

        protected override void OnRenderLabelBackground(ToolStripItemRenderEventArgs e)
        {
            using (var brush = new SolidBrush(e.Item.BackColor))
            {
                e.Graphics.FillRectangle(brush, new Rectangle(Point.Empty, e.Item.Size));
            }
        }
    }

    public class ModifiedColorsLightTheme : ProfessionalColorTable
    {

    }

    public class ModifiedColorsDarkTheme : ProfessionalColorTable
    {
        //Sets left border color
        public override Color ImageMarginGradientBegin
        {
            get { return StringsAndConstants.DARK_BACKCOLOR; }
        }

        //Sets dropdown menu background color
        public override Color MenuItemSelected
        {
            get { return StringsAndConstants.DARK_BACKCOLOR; }
        }

        //Sets dropdown menu background color
        public override Color MenuItemBorder
        {
            get { return StringsAndConstants.DARK_BACKCOLOR; }
        }

        //Sets dropdown menu border color
        public override Color ToolStripDropDownBackground
        {
            get { return StringsAndConstants.DARK_BACKCOLOR; }
        }

        //Sets dropdown menu border color
        public override Color MenuBorder
        {
            get { return StringsAndConstants.DARK_BACKCOLOR; }
        }

        //Sets menu button border color
        public override Color ButtonSelectedBorder
        {
            get { return StringsAndConstants.DARK_BACKCOLOR; }
        }

        //Sets menu button up color
        public override Color ButtonSelectedGradientBegin
        {
            get { return StringsAndConstants.DARK_BACKCOLOR; }
        }

        //Sets menu button down color
        public override Color ButtonSelectedGradientEnd
        {
            get { return StringsAndConstants.DARK_BACKCOLOR; }
        }

        //Sets dropdown menu up color when pressed
        public override Color MenuItemPressedGradientBegin
        {
            get { return StringsAndConstants.PRESSED_STRIP_BUTTON; }
        }

        //Sets dropdown menu up color when pressed
        public override Color MenuItemPressedGradientEnd
        {
            get { return StringsAndConstants.PRESSED_STRIP_BUTTON; }
        }
    }
}
