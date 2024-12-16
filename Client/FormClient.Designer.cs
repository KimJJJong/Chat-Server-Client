namespace Client
{
    partial class FormClient
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            label2 = new Label();
            textIP = new TextBox();
            textPort = new TextBox();
            buttonConnect = new Button();
            textMessage = new TextBox();
            textInput = new TextBox();
            buttonSend = new Button();
            buttonClose = new Button();
            label3 = new Label();
            textID = new TextBox();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(34, 32);
            label1.TabIndex = 0;
            label1.Text = "IP";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(311, 9);
            label2.Name = "label2";
            label2.Size = new Size(75, 32);
            label2.TabIndex = 1;
            label2.Text = "PORT";
            // 
            // textIP
            // 
            textIP.Location = new Point(96, 6);
            textIP.Name = "textIP";
            textIP.Size = new Size(200, 39);
            textIP.TabIndex = 2;
            // 
            // textPort
            // 
            textPort.Location = new Point(395, 6);
            textPort.Name = "textPort";
            textPort.Size = new Size(200, 39);
            textPort.TabIndex = 3;
            // 
            // buttonConnect
            // 
            buttonConnect.Location = new Point(896, 9);
            buttonConnect.Name = "buttonConnect";
            buttonConnect.Size = new Size(150, 46);
            buttonConnect.TabIndex = 4;
            buttonConnect.Text = "접속";
            buttonConnect.UseVisualStyleBackColor = true;
            buttonConnect.Click += buttonConnect_Click;
            // 
            // textMessage
            // 
            textMessage.Location = new Point(30, 73);
            textMessage.Multiline = true;
            textMessage.Name = "textMessage";
            textMessage.Size = new Size(1289, 767);
            textMessage.TabIndex = 5;
            // 
            // textInput
            // 
            textInput.Location = new Point(30, 877);
            textInput.Name = "textInput";
            textInput.Size = new Size(1121, 39);
            textInput.TabIndex = 6;
            textInput.KeyDown += textInput_KeyDown;
            // 
            // buttonSend
            // 
            buttonSend.Location = new Point(1169, 873);
            buttonSend.Name = "buttonSend";
            buttonSend.Size = new Size(150, 46);
            buttonSend.TabIndex = 7;
            buttonSend.Text = "전송";
            buttonSend.UseVisualStyleBackColor = true;
            buttonSend.Click += buttonSend_Click;
            // 
            // buttonClose
            // 
            buttonClose.Location = new Point(1064, 6);
            buttonClose.Name = "buttonClose";
            buttonClose.Size = new Size(146, 46);
            buttonClose.TabIndex = 8;
            buttonClose.Text = "종료";
            buttonClose.UseVisualStyleBackColor = true;
            buttonClose.Click += buttonClose_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(615, 9);
            label3.Name = "label3";
            label3.Size = new Size(37, 32);
            label3.TabIndex = 9;
            label3.Text = "ID";
            // 
            // textID
            // 
            textID.Location = new Point(658, 9);
            textID.Name = "textID";
            textID.Size = new Size(200, 39);
            textID.TabIndex = 10;
            // 
            // FormClient
            // 
            AutoScaleDimensions = new SizeF(14F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1338, 945);
            Controls.Add(textID);
            Controls.Add(label3);
            Controls.Add(buttonClose);
            Controls.Add(buttonSend);
            Controls.Add(textInput);
            Controls.Add(textMessage);
            Controls.Add(buttonConnect);
            Controls.Add(textPort);
            Controls.Add(textIP);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "FormClient";
            Text = "FormClient";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private TextBox textIP;
        private TextBox textPort;
        private Button buttonConnect;
        private TextBox textMessage;
        private TextBox textInput;
        private Button buttonSend;
        private Button buttonClose;
        private Label label3;
        private TextBox textID;
    }
}
