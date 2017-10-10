using System;
using System.Windows.Forms;

namespace HardwareInformation
{
   public partial class Form1 : Form
   {
      public Form1()
      {
         InitializeComponent();
      }

      private void InitializeComponent()
      {
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
         this.lblBM = new System.Windows.Forms.Label();
         this.lblModel = new System.Windows.Forms.Label();
         this.lblSerialNo = new System.Windows.Forms.Label();
         this.lblProcName = new System.Windows.Forms.Label();
         this.lblPM = new System.Windows.Forms.Label();
         this.lblHDSize = new System.Windows.Forms.Label();
         this.lblOS = new System.Windows.Forms.Label();
         this.lblHostname = new System.Windows.Forms.Label();
         this.lblMac = new System.Windows.Forms.Label();
         this.lblIP = new System.Windows.Forms.Label();
         this.coleta = new System.Windows.Forms.Button();
         this.label1 = new System.Windows.Forms.Label();
         this.label2 = new System.Windows.Forms.Label();
         this.label3 = new System.Windows.Forms.Label();
         this.label4 = new System.Windows.Forms.Label();
         this.label5 = new System.Windows.Forms.Label();
         this.label6 = new System.Windows.Forms.Label();
         this.label7 = new System.Windows.Forms.Label();
         this.label8 = new System.Windows.Forms.Label();
         this.label9 = new System.Windows.Forms.Label();
         this.label10 = new System.Windows.Forms.Label();
         this.label11 = new System.Windows.Forms.Label();
         this.label12 = new System.Windows.Forms.Label();
         this.label13 = new System.Windows.Forms.Label();
         this.textBox1 = new System.Windows.Forms.TextBox();
         this.textBox2 = new System.Windows.Forms.TextBox();
         this.textBox3 = new System.Windows.Forms.TextBox();
         this.label14 = new System.Windows.Forms.Label();
         this.label15 = new System.Windows.Forms.Label();
         this.label16 = new System.Windows.Forms.Label();
         this.comboBox1 = new System.Windows.Forms.ComboBox();
         this.comboBox2 = new System.Windows.Forms.ComboBox();
         this.monthCalendar1 = new System.Windows.Forms.MonthCalendar();
         this.webBrowser1 = new System.Windows.Forms.WebBrowser();
         this.label17 = new System.Windows.Forms.Label();
         this.comboBox3 = new System.Windows.Forms.ComboBox();
         this.cadastra = new System.Windows.Forms.Button();
         this.label18 = new System.Windows.Forms.Label();
         this.comboBox4 = new System.Windows.Forms.ComboBox();
         this.label19 = new System.Windows.Forms.Label();
         this.comboBox5 = new System.Windows.Forms.ComboBox();
         this.SuspendLayout();
         // 
         // lblBM
         // 
         this.lblBM.AutoSize = true;
         this.lblBM.Location = new System.Drawing.Point(182, 20);
         this.lblBM.Name = "lblBM";
         this.lblBM.Size = new System.Drawing.Size(0, 13);
         this.lblBM.TabIndex = 7;
         // 
         // lblModel
         // 
         this.lblModel.AutoSize = true;
         this.lblModel.Location = new System.Drawing.Point(182, 49);
         this.lblModel.Name = "lblModel";
         this.lblModel.Size = new System.Drawing.Size(0, 13);
         this.lblModel.TabIndex = 8;
         // 
         // lblSerialNo
         // 
         this.lblSerialNo.AutoSize = true;
         this.lblSerialNo.Location = new System.Drawing.Point(182, 78);
         this.lblSerialNo.Name = "lblSerialNo";
         this.lblSerialNo.Size = new System.Drawing.Size(0, 13);
         this.lblSerialNo.TabIndex = 9;
         // 
         // lblProcName
         // 
         this.lblProcName.AutoSize = true;
         this.lblProcName.Location = new System.Drawing.Point(182, 107);
         this.lblProcName.Name = "lblProcName";
         this.lblProcName.Size = new System.Drawing.Size(0, 13);
         this.lblProcName.TabIndex = 10;
         // 
         // lblPM
         // 
         this.lblPM.AutoSize = true;
         this.lblPM.Location = new System.Drawing.Point(182, 136);
         this.lblPM.Name = "lblPM";
         this.lblPM.Size = new System.Drawing.Size(0, 13);
         this.lblPM.TabIndex = 11;
         // 
         // lblHDSize
         // 
         this.lblHDSize.AutoSize = true;
         this.lblHDSize.Location = new System.Drawing.Point(182, 165);
         this.lblHDSize.Name = "lblHDSize";
         this.lblHDSize.Size = new System.Drawing.Size(0, 13);
         this.lblHDSize.TabIndex = 12;
         // 
         // lblOS
         // 
         this.lblOS.AutoSize = true;
         this.lblOS.Location = new System.Drawing.Point(182, 194);
         this.lblOS.Name = "lblOS";
         this.lblOS.Size = new System.Drawing.Size(0, 13);
         this.lblOS.TabIndex = 13;
         // 
         // lblHostname
         // 
         this.lblHostname.AutoSize = true;
         this.lblHostname.Location = new System.Drawing.Point(182, 223);
         this.lblHostname.Name = "lblHostname";
         this.lblHostname.Size = new System.Drawing.Size(0, 13);
         this.lblHostname.TabIndex = 15;
         // 
         // lblMac
         // 
         this.lblMac.AutoSize = true;
         this.lblMac.Location = new System.Drawing.Point(182, 252);
         this.lblMac.Name = "lblMac";
         this.lblMac.Size = new System.Drawing.Size(0, 13);
         this.lblMac.TabIndex = 18;
         // 
         // lblIP
         // 
         this.lblIP.AutoSize = true;
         this.lblIP.Location = new System.Drawing.Point(182, 280);
         this.lblIP.Name = "lblIP";
         this.lblIP.Size = new System.Drawing.Size(0, 13);
         this.lblIP.TabIndex = 19;
         // 
         // coleta
         // 
         this.coleta.Location = new System.Drawing.Point(145, 309);
         this.coleta.Name = "coleta";
         this.coleta.Size = new System.Drawing.Size(204, 29);
         this.coleta.TabIndex = 20;
         this.coleta.Text = "Coletar hardware";
         this.coleta.UseVisualStyleBackColor = true;
         this.coleta.Click += new System.EventHandler(this.coleta_Click);
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point(27, 19);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(40, 13);
         this.label1.TabIndex = 21;
         this.label1.Text = "Marca:";
         // 
         // label2
         // 
         this.label2.AutoSize = true;
         this.label2.Location = new System.Drawing.Point(27, 49);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(45, 13);
         this.label2.TabIndex = 22;
         this.label2.Text = "Modelo:";
         // 
         // label3
         // 
         this.label3.AutoSize = true;
         this.label3.Location = new System.Drawing.Point(27, 78);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(76, 13);
         this.label3.TabIndex = 23;
         this.label3.Text = "Número Serial:";
         // 
         // label4
         // 
         this.label4.AutoSize = true;
         this.label4.Location = new System.Drawing.Point(27, 107);
         this.label4.Name = "label4";
         this.label4.Size = new System.Drawing.Size(69, 13);
         this.label4.TabIndex = 24;
         this.label4.Text = "Processador:";
         // 
         // label5
         // 
         this.label5.AutoSize = true;
         this.label5.Location = new System.Drawing.Point(27, 136);
         this.label5.Name = "label5";
         this.label5.Size = new System.Drawing.Size(50, 13);
         this.label5.TabIndex = 25;
         this.label5.Text = "Memória:";
         // 
         // label6
         // 
         this.label6.AutoSize = true;
         this.label6.Location = new System.Drawing.Point(27, 165);
         this.label6.Name = "label6";
         this.label6.Size = new System.Drawing.Size(26, 13);
         this.label6.TabIndex = 26;
         this.label6.Text = "HD:";
         // 
         // label7
         // 
         this.label7.AutoSize = true;
         this.label7.Location = new System.Drawing.Point(27, 194);
         this.label7.Name = "label7";
         this.label7.Size = new System.Drawing.Size(107, 13);
         this.label7.TabIndex = 27;
         this.label7.Text = "Sistema Operacional:";
         // 
         // label8
         // 
         this.label8.AutoSize = true;
         this.label8.Location = new System.Drawing.Point(27, 223);
         this.label8.Name = "label8";
         this.label8.Size = new System.Drawing.Size(113, 13);
         this.label8.TabIndex = 28;
         this.label8.Text = "Nome do Computador:";
         // 
         // label9
         // 
         this.label9.AutoSize = true;
         this.label9.Location = new System.Drawing.Point(27, 252);
         this.label9.Name = "label9";
         this.label9.Size = new System.Drawing.Size(82, 13);
         this.label9.TabIndex = 29;
         this.label9.Text = "Endereço MAC:";
         // 
         // label10
         // 
         this.label10.AutoSize = true;
         this.label10.Location = new System.Drawing.Point(27, 280);
         this.label10.Name = "label10";
         this.label10.Size = new System.Drawing.Size(69, 13);
         this.label10.TabIndex = 30;
         this.label10.Text = "Endereço IP:";
         // 
         // label11
         // 
         this.label11.AutoSize = true;
         this.label11.Location = new System.Drawing.Point(492, 19);
         this.label11.Name = "label11";
         this.label11.Size = new System.Drawing.Size(59, 13);
         this.label11.TabIndex = 31;
         this.label11.Text = "Patrimônio:";
         // 
         // label12
         // 
         this.label12.AutoSize = true;
         this.label12.Location = new System.Drawing.Point(492, 45);
         this.label12.Name = "label12";
         this.label12.Size = new System.Drawing.Size(88, 13);
         this.label12.TabIndex = 32;
         this.label12.Text = "Número do lacre:";
         // 
         // label13
         // 
         this.label13.AutoSize = true;
         this.label13.Location = new System.Drawing.Point(492, 97);
         this.label13.Name = "label13";
         this.label13.Size = new System.Drawing.Size(40, 13);
         this.label13.TabIndex = 33;
         this.label13.Text = "Prédio:";
         // 
         // textBox1
         // 
         this.textBox1.Location = new System.Drawing.Point(606, 16);
         this.textBox1.Name = "textBox1";
         this.textBox1.Size = new System.Drawing.Size(334, 20);
         this.textBox1.TabIndex = 34;
         // 
         // textBox2
         // 
         this.textBox2.Location = new System.Drawing.Point(606, 42);
         this.textBox2.Name = "textBox2";
         this.textBox2.Size = new System.Drawing.Size(334, 20);
         this.textBox2.TabIndex = 35;
         // 
         // textBox3
         // 
         this.textBox3.Location = new System.Drawing.Point(606, 68);
         this.textBox3.Name = "textBox3";
         this.textBox3.Size = new System.Drawing.Size(334, 20);
         this.textBox3.TabIndex = 36;
         // 
         // label14
         // 
         this.label14.AutoSize = true;
         this.label14.Location = new System.Drawing.Point(492, 71);
         this.label14.Name = "label14";
         this.label14.Size = new System.Drawing.Size(31, 13);
         this.label14.TabIndex = 38;
         this.label14.Text = "Sala:";
         // 
         // label15
         // 
         this.label15.AutoSize = true;
         this.label15.Location = new System.Drawing.Point(492, 124);
         this.label15.Name = "label15";
         this.label15.Size = new System.Drawing.Size(97, 13);
         this.label15.TabIndex = 39;
         this.label15.Text = "Cadastrado no AD:";
         // 
         // label16
         // 
         this.label16.AutoSize = true;
         this.label16.Location = new System.Drawing.Point(492, 252);
         this.label16.Name = "label16";
         this.label16.Size = new System.Drawing.Size(95, 13);
         this.label16.TabIndex = 40;
         this.label16.Text = "Última formatação:";
         // 
         // comboBox1
         // 
         this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.comboBox1.FormattingEnabled = true;
         this.comboBox1.Items.AddRange(new object[] {
            "21",
            "67",
            "74 - A",
            "74 - B",
            "74 - C",
            "ANTIGA REITORIA",
            "APOIO",
            "BIBLIOTECA SETORIAL"});
         this.comboBox1.Location = new System.Drawing.Point(606, 94);
         this.comboBox1.Name = "comboBox1";
         this.comboBox1.Size = new System.Drawing.Size(121, 21);
         this.comboBox1.Sorted = true;
         this.comboBox1.TabIndex = 41;
         // 
         // comboBox2
         // 
         this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.comboBox2.FormattingEnabled = true;
         this.comboBox2.Items.AddRange(new object[] {
            "NAO",
            "SIM"});
         this.comboBox2.Location = new System.Drawing.Point(606, 121);
         this.comboBox2.Name = "comboBox2";
         this.comboBox2.Size = new System.Drawing.Size(121, 21);
         this.comboBox2.Sorted = true;
         this.comboBox2.TabIndex = 42;
         // 
         // monthCalendar1
         // 
         this.monthCalendar1.Location = new System.Drawing.Point(606, 176);
         this.monthCalendar1.MaxSelectionCount = 1;
         this.monthCalendar1.Name = "monthCalendar1";
         this.monthCalendar1.TabIndex = 43;
         // 
         // webBrowser1
         // 
         this.webBrowser1.Location = new System.Drawing.Point(13, 350);
         this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
         this.webBrowser1.Name = "webBrowser1";
         this.webBrowser1.Size = new System.Drawing.Size(928, 103);
         this.webBrowser1.TabIndex = 44;
         // 
         // label17
         // 
         this.label17.AutoSize = true;
         this.label17.Location = new System.Drawing.Point(492, 152);
         this.label17.Name = "label17";
         this.label17.Size = new System.Drawing.Size(44, 13);
         this.label17.TabIndex = 45;
         this.label17.Text = "Padrão:";
         // 
         // comboBox3
         // 
         this.comboBox3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.comboBox3.FormattingEnabled = true;
         this.comboBox3.Items.AddRange(new object[] {
            "AD",
            "PCCLI"});
         this.comboBox3.Location = new System.Drawing.Point(606, 149);
         this.comboBox3.Name = "comboBox3";
         this.comboBox3.Size = new System.Drawing.Size(121, 21);
         this.comboBox3.TabIndex = 46;
         // 
         // cadastra
         // 
         this.cadastra.Location = new System.Drawing.Point(330, 459);
         this.cadastra.Name = "cadastra";
         this.cadastra.Size = new System.Drawing.Size(278, 63);
         this.cadastra.TabIndex = 47;
         this.cadastra.Text = "Cadastrar dados";
         this.cadastra.UseVisualStyleBackColor = true;
         this.cadastra.Click += new System.EventHandler(this.cadastra_Click);
         // 
         // label18
         // 
         this.label18.AutoSize = true;
         this.label18.Location = new System.Drawing.Point(758, 97);
         this.label18.Name = "label18";
         this.label18.Size = new System.Drawing.Size(45, 13);
         this.label18.TabIndex = 48;
         this.label18.Text = "Em uso:";
         // 
         // comboBox4
         // 
         this.comboBox4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.comboBox4.FormattingEnabled = true;
         this.comboBox4.Items.AddRange(new object[] {
            "NAO",
            "SIM"});
         this.comboBox4.Location = new System.Drawing.Point(819, 94);
         this.comboBox4.Name = "comboBox4";
         this.comboBox4.Size = new System.Drawing.Size(121, 21);
         this.comboBox4.TabIndex = 49;
         // 
         // label19
         // 
         this.label19.AutoSize = true;
         this.label19.Location = new System.Drawing.Point(758, 123);
         this.label19.Name = "label19";
         this.label19.Size = new System.Drawing.Size(49, 13);
         this.label19.TabIndex = 50;
         this.label19.Text = "Etiqueta:";
         // 
         // comboBox5
         // 
         this.comboBox5.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.comboBox5.FormattingEnabled = true;
         this.comboBox5.Items.AddRange(new object[] {
            "NAO",
            "SIM"});
         this.comboBox5.Location = new System.Drawing.Point(819, 120);
         this.comboBox5.Name = "comboBox5";
         this.comboBox5.Size = new System.Drawing.Size(121, 21);
         this.comboBox5.TabIndex = 51;
         // 
         // Form1
         // 
         this.AutoScroll = true;
         this.AutoSize = true;
         this.ClientSize = new System.Drawing.Size(953, 528);
         this.Controls.Add(this.comboBox5);
         this.Controls.Add(this.label19);
         this.Controls.Add(this.comboBox4);
         this.Controls.Add(this.label18);
         this.Controls.Add(this.cadastra);
         this.Controls.Add(this.comboBox3);
         this.Controls.Add(this.label17);
         this.Controls.Add(this.webBrowser1);
         this.Controls.Add(this.monthCalendar1);
         this.Controls.Add(this.comboBox2);
         this.Controls.Add(this.comboBox1);
         this.Controls.Add(this.label16);
         this.Controls.Add(this.label15);
         this.Controls.Add(this.label14);
         this.Controls.Add(this.textBox3);
         this.Controls.Add(this.textBox2);
         this.Controls.Add(this.textBox1);
         this.Controls.Add(this.label13);
         this.Controls.Add(this.label12);
         this.Controls.Add(this.label11);
         this.Controls.Add(this.label10);
         this.Controls.Add(this.label9);
         this.Controls.Add(this.label8);
         this.Controls.Add(this.label7);
         this.Controls.Add(this.label6);
         this.Controls.Add(this.label5);
         this.Controls.Add(this.label4);
         this.Controls.Add(this.label3);
         this.Controls.Add(this.label2);
         this.Controls.Add(this.label1);
         this.Controls.Add(this.coleta);
         this.Controls.Add(this.lblIP);
         this.Controls.Add(this.lblMac);
         this.Controls.Add(this.lblHostname);
         this.Controls.Add(this.lblBM);
         this.Controls.Add(this.lblModel);
         this.Controls.Add(this.lblSerialNo);
         this.Controls.Add(this.lblProcName);
         this.Controls.Add(this.lblPM);
         this.Controls.Add(this.lblHDSize);
         this.Controls.Add(this.lblOS);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.MaximizeBox = false;
         this.Name = "Form1";
         this.Text = "Coleta e cadastro de hardware / CCSH - UFSM";
         this.Load += new System.EventHandler(this.Form1_Load);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      private Label lblBM;
      private Label lblModel;
      private Label lblSerialNo;
      private Label lblProcName;
      private Label lblPM;
      private Label lblHDSize;
      private Label lblHostname;
      private Label lblMac;
      private Label lblIP;
      private Button coleta;
      private Label label1;
      private Label label2;
      private Label label3;
      private Label label4;
      private Label label5;
      private Label label6;
      private Label label7;
      private Label label8;
      private Label label9;
      private Label label10;
      private Label label11;
      private Label label12;
      private Label label13;
      private TextBox textBox1;
      private TextBox textBox2;
      private TextBox textBox3;
      private Label label14;
      private Label label15;
      private Label label16;
      private ComboBox comboBox1;
      private ComboBox comboBox2;
      private MonthCalendar monthCalendar1;
      private WebBrowser webBrowser1;
      private Label lblOS;     
      private Label label17;
      private ComboBox comboBox3;
      private Label label18;
      private ComboBox comboBox4;
      private Label label19;
      private ComboBox comboBox5;
      private Button cadastra;
      private bool flag = false;

      private string varPatrimonio, varLacre, varSala, varBoard, varModel,
         varSerial, varProc, varRAM, varHD, varOS, varHostname, varMac,
         varIP, varPredio, varCadastrado, varPadrao, varCalend, varUso, varTag;

      private void Form1_Load(object sender, EventArgs e)
      {
         comboBox1.SelectedItem = "74 - C";
         comboBox2.SelectedItem = "NAO";
         comboBox3.SelectedItem = "PCCLI";
         comboBox4.SelectedItem = "SIM";
         comboBox5.SelectedItem = "NAO";
      }

      private void coleta_Click(object sender, EventArgs e)
      {
         flag = true;
         varBoard = lblBM.Text = HardwareInfo.GetBoardMaker();
         varModel = lblModel.Text = HardwareInfo.GetModel();
         varSerial = lblSerialNo.Text = HardwareInfo.GetBoardProductId();
         varProc = lblProcName.Text = HardwareInfo.GetProcessorInfo();
         varRAM = lblPM.Text = HardwareInfo.GetPhysicalMemory();
         varHD = lblHDSize.Text = HardwareInfo.GetHDSize();
         varOS = lblOS.Text = HardwareInfo.GetOSInformation();
         varHostname = lblHostname.Text = HardwareInfo.GetComputerName();
         varMac = lblMac.Text = HardwareInfo.GetMACAddress();
         varIP = lblIP.Text = HardwareInfo.GetIPAddress();
      }

      private void cadastra_Click(object sender, EventArgs e)
      {
         if (!string.IsNullOrWhiteSpace(textBox1.Text) && !string.IsNullOrWhiteSpace(textBox3.Text))
         {
            if (flag == true)
            {
               varPatrimonio = textBox1.Text;
               varLacre = textBox2.Text;
               varSala = textBox3.Text;
               varPredio = comboBox1.SelectedItem.ToString();
               varCadastrado = comboBox2.SelectedItem.ToString();
               varPadrao = comboBox3.SelectedItem.ToString();
               varUso = comboBox4.SelectedItem.ToString();
               varTag = comboBox5.SelectedItem.ToString();
               varCalend = monthCalendar1.SelectionRange.Start.ToString();

               webBrowser1.Navigate("http://192.168.76.103:8081/recebeDados.php?patrimonio=" + varPatrimonio + "&lacre=" + varLacre +
                  "&sala=" + varSala + "&predio=" + varPredio + "&ad=" + varCadastrado + "&padrao=" + varPadrao + "&formatacao=" + varCalend +
                  "&marca=" + varBoard + "&modelo=" + varModel + "&numeroSerial=" + varSerial + "&processador=" + varProc + "&memoria=" + varRAM +
                  "&hd=" + varHD + "&sistemaOperacional=" + varOS + "&nomeDoComputador=" + varHostname + "&mac=" + varMac + "&ip=" + varIP + "&emUso=" + varUso +
                  "&etiqueta=" + varTag);
            }
            else
            {
               MessageBox.Show("Clique em 'Coletar Hardware'", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
         }
         else
         {            
            MessageBox.Show("Preencha os campos 'Patrimônio' e 'Sala'", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }
   }
}
