using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Net.Mail;
using System.Threading;
using System.Media;
//using Microsoft.

namespace Крестики_нолики
{
    public partial class Main_window : Form
    {
        public Main_window()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop(); //выключаем таймер, чтобы больше не включался
            lo.Visible = false; //выключаем заставку
            lo = null; //отчищаем память от нее
            tableLayoutPanel2.Visible = true; //включаем обратно интерфейс приложения
        }
        public static PictureBox lo = new PictureBox(); //бокс для загрузочного изображения
        static int[] lol; //массив отвечает за поле. 3 - пустая ячейка, 1 - стоит крестик, 0 - стоит нолик
        static bool hod = true; //переменная для локальной игры определяет какой игрок сейчас ходит; в сетевой игре hod будет отвечать за возможность хода
        static Bitmap plus, zero; //изображения крестика и нолика
        static bool local_or_ethernet = true; //локальная игра сетевая игра
        static bool local_comp = true; //если правда в локальной игре участвует компьютер
        static bool local_comp_block = false; //если правда то блокирует игрока
        static bool gamer = true; //если правда то при сетевой игре играет данный клиент за крестика 
        static Socket soc, soc_mess; //используемые сокеты для подключений
        //SoundPlayer mus = new SoundPlayer(Resource1.Space_Dreams_spaces_ru); //подключение звукового файла/потока к переменной фонового звука
        bool music_play_sound = true; //если правда, то сейчас играет музыка, если нет, то музыка не играет.

        private void Form1_Load(object sender, EventArgs e)
        {
            lol = new int[9]; //выделение размерности массива
            for (int i = 0; i < 9; i++)
                lol[i] = 3; //установка того, что поле пустое, изображения по умолчанию пустые поэтому их отчищать не нужно
            plus = new Bitmap(Resource1.plusic); //подключение изображения крестика
            zero = new Bitmap(Resource1.nulik); //подключение изображения нолика
            now_hod_player.Text = "Ход крестика"; //по умолчанию ходить будет крестик, в сетевой игре для игрока играющего за нолик этот параметр можно изменить
            type_game.Text = "Локальная игра"; //по умолчанию начинается локальная игра в соответствии с нашей переменной
            lo.Image = Resource1.load_picture_image; //реализация заставки
            lo.Parent = tableLayoutPanel2.Parent;
            lo.SizeMode = PictureBoxSizeMode.Zoom;
            lo.Dock = DockStyle.Fill; //заполнонить форму заставкой
            tableLayoutPanel2.Visible = false; //убираем с нашей формы все, что есть, чтобы можно было сделать видимой заставку
            lo.Visible = true; //показываем нашу заставку предварительно в предыдущей строчке убрав все остальное с формы
            timer1.Start(); //включение таймера, который потом выключит заставку и выключит заставка
            pic_mus_play_pause_stop.Image = Resource1.play_pic; music_play_sound = true; //mus.PlayLooping(); //по умолчанию воспроизведение нашей фоновой мущыке
            tableLayoutPanel2.BackColor = Color.White; //делаем все поле белым, чтобы не было видно белых частей картинок
            new Thread(delegate () { my_ip(false); }).Start(); //запускаем в отдельном потоке определение внутренего и внешнего адресов false - вншений ip
            new Thread(delegate () { my_ip(true); }).Start(); //запуск в отдельеном потоке определения внутрнего и внешнего адресов true - внутренний ip
        }
        
        private void loading_pic()
        { 
            Thread.Sleep(1000);
            lo.Visible = false;
            lo = null;
            tableLayoutPanel2.Visible = true;
        } /* данная функция должна была использоваться, чтобы отображать заставку, 
        но в потоке нельзя управлять элементами управления созданными другим потоком 
        у меня было несколько идей на тему того как это обойти, но не вижу смысла сильно заморачиваться над этой проблемой
        Все таки не 10 секундую заставку я отображаю, а просто картинку на секунду для красоты*/

        private void my_ip(bool ip_t)
        {
            if (Socket.OSSupportsIPv4 == true && ip_t == false)
            {
                try
                {
                    string ip = new WebClient().DownloadString("http://checkip.dyndns.com/");
                    int i1 = ip.IndexOf("Current IP Address: ") + 20;
                    int i2 = ip.IndexOf("</body>");
                    ip = ip.Remove(i2);
                    ip = ip.Remove(0, i1);
                    IPAddress ipad;
                    if (IPAddress.TryParse(ip, out ipad) == true)
                    {
                        ip_eth_label.Text = "Внешний ip: " + ipad.ToString() + "  ";
                    }
                    else
                    {
                        ip_eth_label.Text = "";
                    }
                }
                catch (Exception ex)
                {
                    new Mail_game_info().error(ex);
                }
            }
            else if (Socket.OSSupportsIPv4 == true && ip_t == true)
            {
                IPAddress[] ipa = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
                for (int i = 0; i < ipa.Length; i++)
                {
                    if (ipa[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        ip_local_label.Text = "Внутренний ip: " + ipa[i].ToString() + "  ";
                    }
                }
            }
        }

        public void ed() //проверка поля на ничью игру или победу того или иного игровка.
        {
            bool fin = false;
            if ((lol[0] == 1 && lol[1] == 1 && lol[2] == 1) ||
                    (lol[3] == 1 && lol[4] == 1 && lol[5] == 1) ||
                    (lol[6] == 1 && lol[7] == 1 && lol[8] == 1) ||
                    (lol[6] == 1 && lol[3] == 1 && lol[0] == 1) ||
                    (lol[1] == 1 && lol[4] == 1 && lol[7] == 1) ||
                    (lol[2] == 1 && lol[5] == 1 && lol[8] == 1) ||
                    (lol[0] == 1 && lol[4] == 1 && lol[8] == 1) ||
                    (lol[2] == 1 && lol[4] == 1 && lol[6] == 1))
            { //проверям поле на наличие победы крестика
                DialogResult ms = MessageBox.Show("Игрок крестик победил.\nНачать новую игру?", "Победа", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (ms == DialogResult.Yes)
                {
                    fin = true;
                    if (gamer && local_or_ethernet == false) Properties.Settings.Default.Game_Win++;
                    else if (gamer == false && local_or_ethernet == false) Properties.Settings.Default.Game_over++;
                    else if (comp_kr_or_null == 0 && local_comp && local_or_ethernet) { Properties.Settings.Default.Game_Win++; new Mail_game_info().info_send(jnral_hod); jnral_hod = ""; }
                    else if (comp_kr_or_null == 1 && local_comp && local_or_ethernet) { Properties.Settings.Default.Game_over++; }
                    Properties.Settings.Default.Save(); comp_kr_or_null = -1;
                }
                else if (ms == DialogResult.No)
                {
                    if (gamer && local_or_ethernet == false) Properties.Settings.Default.Game_Win++;
                    else if (gamer == false && local_or_ethernet == false) Properties.Settings.Default.Game_over++;
                    else if (comp_kr_or_null == 0 && local_comp && local_or_ethernet) { Properties.Settings.Default.Game_Win++; new Mail_game_info().info_send(jnral_hod); jnral_hod = ""; }
                    else if (comp_kr_or_null == 1 && local_comp && local_or_ethernet) { Properties.Settings.Default.Game_over++; }
                    Properties.Settings.Default.Save(); comp_kr_or_null = -1;
                    try
                    {
                        soc.Close();
                    }
                    catch (Exception ex)
                    {
                        new Mail_game_info().error(ex);
                    }
                    this.Close();
                }
            } //проверяем поле на наличие победы нолика
            else if ((lol[0] == 0 && lol[1] == 0 && lol[2] == 0) ||
                (lol[3] == 0 && lol[4] == 0 && lol[5] == 0) ||
                (lol[6] == 0 && lol[7] == 0 && lol[8] == 0) ||
                (lol[6] == 0 && lol[3] == 0 && lol[0] == 0) ||
                (lol[1] == 0 && lol[4] == 0 && lol[7] == 0) ||
                (lol[2] == 0 && lol[5] == 0 && lol[8] == 0) ||
                (lol[0] == 0 && lol[4] == 0 && lol[8] == 0) ||
                (lol[2] == 0 && lol[4] == 0 && lol[6] == 0))
            {
                DialogResult ms = MessageBox.Show("Игрок нолик победил.\nНачать новую игру?", "Победа", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (ms == DialogResult.Yes)
                {
                    fin = true;
                    if (gamer == false && local_or_ethernet == false) Properties.Settings.Default.Game_Win++;
                    else if (gamer && local_or_ethernet == false) Properties.Settings.Default.Game_over++;
                    else if (comp_kr_or_null == 1 && local_comp && local_or_ethernet) { Properties.Settings.Default.Game_Win++; new Mail_game_info().info_send(jnral_hod); jnral_hod = ""; }
                    else if (comp_kr_or_null == 0 && local_comp && local_or_ethernet) Properties.Settings.Default.Game_over++;
                    Properties.Settings.Default.Save(); comp_kr_or_null = -1;
                }
                else if (ms == DialogResult.No)
                {
                    if (gamer == false && local_or_ethernet == false) Properties.Settings.Default.Game_Win++;
                    else if (gamer && local_or_ethernet == false) Properties.Settings.Default.Game_over++;
                    else if (comp_kr_or_null == 1 && local_comp && local_or_ethernet) { Properties.Settings.Default.Game_Win++; new Mail_game_info().info_send(jnral_hod); jnral_hod = ""; }
                    else if (comp_kr_or_null == 0 && local_comp && local_or_ethernet) Properties.Settings.Default.Game_over++;
                    Properties.Settings.Default.Save(); comp_kr_or_null = -1;
                    try
                    {
                        soc.Close();
                    }
                    catch (Exception ex)
                    {
                        new Mail_game_info().error(ex);
                    }
                    this.Close();
                }
            }
            else
            { //проверка поле на то, что оно заполнено
                bool ok = true;
                for (int i = 0; i < 9; i++)
                {
                    if (lol[i] == 3) { ok = false; break; } //проверка, что все ячейки заняты
                }
                if (ok) //выводим преложение пользователю
                {
                    Properties.Settings.Default.Game_draw++;
                    Properties.Settings.Default.Save();
                    MessageBox.Show("Поле заполнено. Начнется новая игра.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    fin = true;
                }
            }
            if (fin) //обнуляем все поле и выдаем ход крестику или нолику в зависимости от режима.
            {
                comp_kr_or_null = -1;
                pictureBox1.Image = null;
                pictureBox2.Image = null;
                pictureBox3.Image = null;
                pictureBox4.Image = null;
                pictureBox5.Image = null;
                pictureBox6.Image = null;
                pictureBox7.Image = null;
                pictureBox8.Image = null;
                pictureBox9.Image = null;
                for (int i = 0; i < 9; i++)
                {
                    lol[i] = 3;
                }
                if (local_or_ethernet)
                {
                    hod = true;
                    now_hod_player.Text = "Ход крестика";
                }
                else if (gamer)
                {
                    hod = true;
                    now_hod_player.Text = "Ход крестика";
                }
                else
                {
                    hod = false;
                    now_hod_player.Text = "Ожидание хода крестика";
                }
            }
        }

        private void новаяИграToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void obrabotchik_eth_hod(int lolik, bool kr_or_null)
        {
            if (kr_or_null)
            {
                lol[lolik] = 1;
                if (lolik == 0)
                {
                    pictureBox1.Image = plus;
                }
                else if (lolik == 1)
                {
                    pictureBox2.Image = plus;
                }
                else if (lolik == 2)
                {
                    pictureBox3.Image = plus;
                }
                else if (lolik == 3)
                {
                    pictureBox4.Image = plus;
                }
                else if (lolik == 4)
                {
                    pictureBox5.Image = plus;
                }
                else if (lolik == 5)
                {
                    pictureBox6.Image = plus;
                }
                else if (lolik == 6)
                {
                    pictureBox7.Image = plus;
                }
                else if (lolik == 7)
                {
                    pictureBox8.Image = plus;
                }
                else if (lolik == 8)
                {
                    pictureBox9.Image = plus;
                }
                else
                {
                    MessageBox.Show("Ошибка. Правельный номер ячейки получен от соперника");
                }
            }
            else
            {
                lol[lolik] = 0;
                if (lolik == 0)
                {
                    pictureBox1.Image = zero;
                }
                else if (lolik == 1)
                {
                    pictureBox2.Image = zero;
                }
                else if (lolik == 2)
                {
                    pictureBox3.Image = zero;
                }
                else if (lolik == 3)
                {
                    pictureBox4.Image = zero;
                }
                else if (lolik == 4)
                {
                    pictureBox5.Image = zero;
                }
                else if (lolik == 5)
                {
                    pictureBox6.Image = zero;
                }
                else if (lolik == 6)
                {
                    pictureBox7.Image = zero;
                }
                else if (lolik == 7)
                {
                    pictureBox8.Image = zero;
                }
                else if (lolik == 8)
                {
                    pictureBox9.Image = zero;
                }
                else
                {
                    MessageBox.Show("Ошибка. Правельный номер ячейки получен от соперника");
                }
            }
            hod = true; ed();
            if (gamer)
                now_hod_player.Text = "Ваш ход";
            else
                now_hod_player.Text = "Ожидание хода крестика";
        }

        private void obmen_serv()
        { //первый байт (крестик или нолик), int номер ячейки, сообщение само если нужно.
            while (true)
            {
                try
                {
                    byte[] msg = new byte[soc_mess.Available];
                    soc_mess.Receive(msg);
                    if (msg.Length > 0)
                    {
                        string str = Encoding.UTF8.GetString(msg);
                        if (1 == Int32.Parse(str[0].ToString()))
                        {
                            /*new Main_window().*/obrabotchik_eth_hod(int.Parse(str[1].ToString()), true);
                        }
                        else if (0 == Int32.Parse(str[0].ToString()))
                        {
                           obrabotchik_eth_hod(int.Parse(str[1].ToString()), false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message == "Удаленный хост принудительно разорвал существующее подключение")
                    {
                        try { soc_mess.Close(); } catch (Exception err) { err.ToString(); }
                        try { soc.Close(); } catch (Exception err) { err.ToString(); }
                    }
                    else
                    {
                        MessageBox.Show(ex.ToString(), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    new Mail_game_info().error(ex);
                    goto exit;
                }
            }
            exit:;
        }

        private void obmen_client()
        { //первый байт (крестик или нолик), int номер ячейки, сообщение само если нужно.
            while /*(!Console.KeyAvailable)*/ (true)
            {
                try
                {
                    byte[] msg = new byte[soc.Available];
                    soc.Receive(msg);
                    if (msg.Length > 0)
                    {
                        string str = Encoding.UTF8.GetString(msg);
                        if (1 == Int32.Parse(str[0].ToString()))
                        {
                            obrabotchik_eth_hod(int.Parse(str[1].ToString()), true);
                        }
                        else if (0 == Int32.Parse(str[0].ToString()))
                        {
                            obrabotchik_eth_hod(int.Parse(str[1].ToString()), false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message == "Программа на вашем хост-компьютере разорвала установленное подключение")
                    {
                        try { soc_mess.Close(); } catch (Exception err) { err.ToString(); new Mail_game_info().error(err); }
                        try { soc.Close(); } catch (Exception err) { err.ToString(); new Mail_game_info().error(err); }
                    }
                    else
                    {
                        MessageBox.Show(ex.ToString(), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    new Mail_game_info().error(ex);
                    goto exit;
                }
            }
        exit:;
        }

        private void send_mess(int lolik)
        { //отправка другому клиенту информации о сделанном ходе
            string str; str = null;
            if (gamer) { str = 1.ToString(); } else { str = 0.ToString(); }
            str = str + lolik.ToString();
            byte[] msg = Encoding.UTF8.GetBytes(str);
            if (gamer) { soc_mess.Send(msg); hod = false; } else { soc.Send(msg); hod = false; }
        }

        private void server_creat()
        {
            if (Socket.OSSupportsIPv4 == true && Socket.OSSupportsIPv4 == true)
            {
                connect_info.Text = "Сервер создан. Ожидание подключения клиентов";
                hod = false; //мы сейчас не можем ходить потому что идет загрузка сетевой игры
                local_or_ethernet = false; //переключаем режим на сетевую игру
                type_game.Text = "Ожидание подключения игрока";
                gamer = true; //данный игрок игррает за крестика
                              //ждем вдодящие подключения к сокету

                soc = new Socket(IPAddress.Any.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint adr = new IPEndPoint(IPAddress.Any, 2093);
                soc.Bind(adr);
                soc.Listen(1);
                try
                {
                    soc_mess = soc.Accept();
                }
                catch(Exception ex)
                {
                    if (ex.Message == "Операция блокирования прервана вызовом WSACancelBlockingCall")
                    {

                    }
                    new Mail_game_info().error(ex);
                }
                //обнуляем поле для игры за крестика
                pictureBox1.Image = null;
                pictureBox2.Image = null;
                pictureBox3.Image = null;
                pictureBox4.Image = null;
                pictureBox5.Image = null;
                pictureBox6.Image = null;
                pictureBox7.Image = null;
                pictureBox8.Image = null;
                pictureBox9.Image = null;
                for (int i = 0; i < 9; i++)
                    lol[i] = 3;
                hod = true; now_hod_player.Text = "Ваш ход"; type_game.Text = "Сетевая игра";
                Thread ol = new Thread(delegate () { obmen_serv(); }); ol.Start();
                connect_info.Text = "Подключился клиент. Игра началась";
            }
            else
            {
                connect_info.Text = "Ошибка сетевого подключения";
                MessageBox.Show("Сетевое соединение не поддерживается вашим компьютером. Проверьте настройки сетевого соединения, а затем попробуйте снова.", "Ошибка сети",
                     MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void создатьИгруToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thread sta = new Thread(delegate () { server_creat(); });
            sta.Start();
        }

        private void подключитьсяКИгреToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //int err = 0;
            st:
            if (Socket.OSSupportsIPv4 == true && Socket.OSSupportsIPv4 == true)
            {
                hod = false; //мы сейчас не можем ходить потому что идет загрузка сетевой игры
                local_or_ethernet = false; //переключаем режим на сетевую игру
                gamer = false; //данный игрок игррает за крестика
                               //ждем вдодящие подключения к сокету
                type_game.Text = "Подключение к компьютеру основного игрока";
                soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Connect co = new Connect();
                co.ShowDialog(); IPAddress ipadr = null;
                if (Connect.ip == "" || Connect.ip == null)
                {
                    goto exit;
                }
                else if (Connect.ip.Length < "0.0.0.0".Length || Connect.ip.Length > "244.244.244.254".Length)
                {
                    MessageBox.Show("Неправильный ip адрес. Повторите попытку подключения к серверу", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    goto exit;
                }
                try
                {
                    ipadr = Dns.GetHostAddresses(Connect.ip)[0];
                }
                catch(Exception ex)
                {
                    new Mail_game_info().error(ex);
                    if ("Значение не может быть неопределенным.\r\nИмя параметра: hostNameOrAddress" == ex.Message)
                    {
                        MessageBox.Show("Неправильный ip адрес. Повторите попытку подключения к серверу", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        goto exit;
                    }
                }
                IPEndPoint adr = new IPEndPoint(ipadr, 2093);
            ret:;
                try
                {
                    soc.Connect(adr);
                }
                catch (Exception ex)
                {
                    new Mail_game_info().error(ex);
                    if (ex.Message.StartsWith("Подключение не установлено, т.к. конечный компьютер отверг запрос на подключение 127.0.0.1:2093"))
                    {
                        if (MessageBox.Show("Сервер не запущен или вы неправильно ввели ip сервера. Попробуйте свзяться с пользователем, создавшим сервер, и уточнить ip \n" +
                            "Хотите повторить попытку подключения с установленным ip адресом сервера или ввести новый и повторить попытку?",
                            "Ошибка", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry)
                        {
                            goto ret;
                        }
                        else goto st;
                    }
                    goto exit;
                }
                //обнуляем поле для игры за крестика
                pictureBox1.Image = null;
                pictureBox2.Image = null;
                pictureBox3.Image = null;
                pictureBox4.Image = null;
                pictureBox5.Image = null;
                pictureBox6.Image = null;
                pictureBox7.Image = null;
                pictureBox8.Image = null;
                pictureBox9.Image = null;
                for (int i = 0; i < 9; i++)
                    lol[i] = 3;
                hod = false; now_hod_player.Text = "Ожидание хода крестика"; type_game.Text = "Сетевая игра";
                Thread ol = new Thread(delegate () { obmen_client(); }); ol.Start();
            }
            else
            {
                MessageBox.Show("Сетевое соединение не поддерживается вашим компьютером. Проверьте настройки сетевого соединения, а затем попробуйте снова.", "Ошибка сети",
                     MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        exit:;
        }

        public int obrabotka_hoda(int lolik)
        {
            //local_or_ethernet - локальная правда, сетевая лож
            //gamer - крестик правда, нолик ложь
            //hod - данный игрок может ходить в сетевой игра, в локальной отвечает за ход крестика или нолика
            if (lol[lolik] == 3)
            {
                if (local_or_ethernet) //локальная игра
                {
                    if (hod == true && local_comp_block == false)
                    {
                        hod = false; lol[lolik] = 1; jnral_hod += lolik.ToString() + " ";
                        now_hod_player.Text = "Ход нолика";
                        return 1;
                    }
                    else if (hod == false && local_comp_block == false)
                    {
                        now_hod_player.Text = "Ход крестика";
                        hod = true; lol[lolik] = 0; jnral_hod += lolik.ToString() + " ";
                        return 0;
                    }
                    else if (hod == true && local_comp_block == true)
                    {
                        hod = false; lol[lolik] = 1; jnral_hod += lolik.ToString() + " ";
                        now_hod_player.Text = "Ход нолика";
                        return 1;
                    }
                    else if (hod == false && local_comp_block == true)
                    {
                        now_hod_player.Text = "Ход крестика";
                        hod = true; lol[lolik] = 0; jnral_hod += lolik.ToString() + " ";
                        return 0;
                    }
                }
                else //сетевая игра
                {
                    if (hod)
                    {
                        if (gamer)
                        {
                            lol[lolik] = 1; hod = false; send_mess(lolik); //ed();
                            now_hod_player.Text = "Ожидание хода нолика"; jnral_hod += lolik.ToString() + " ";
                            return 1;
                        }
                        else
                        {
                            lol[lolik] = 0; hod = false; send_mess(lolik); //ed();
                            now_hod_player.Text = "Ожидание хода крестика"; jnral_hod += lolik.ToString() + " ";
                            return 0;
                        }
                    }
                }
            }
            else
            {
                return -1;
            }
            return -2;
        }

        static string jnral_hod = "";
        static int comp_kr_or_null = -1;
        public void comp_hod(int vrah)
        { //в будущем вохможно можно будет сделать данную функцию синхронной, но сейчас в этом нет смысла
            if (local_comp)
            {
                local_comp_block = true;
                Computer comput = new Computer();
                int res = comput.hod_comp_main(lol, vrah);
                if (res < 0 || res > 8) { MessageBox.Show("Ошибка в ходе компьютера. Класс вернул результат - " + res.ToString(), "", MessageBoxButtons.OK, MessageBoxIcon.Error); this.Close(); }
                else
                {
                    if (comp_kr_or_null == -1)
                    {
                        if (hod) { comp_kr_or_null = 1; jnral_hod = "Комп крестик: "; }
                        else { comp_kr_or_null = 0; jnral_hod = "Комп нолик: "; }
                    }
                    int hodok = obrabotka_hoda(res); 
                    if (res == 0) { if (hodok == 1) pictureBox1.Image = plus; else pictureBox1.Image = zero; ed(); }
                    if (res == 1) { if (hodok == 1) pictureBox2.Image = plus; else pictureBox2.Image = zero; ed(); }
                    if (res == 2) { if (hodok == 1) pictureBox3.Image = plus; else pictureBox3.Image = zero; ed(); }
                    if (res == 3) { if (hodok == 1) pictureBox4.Image = plus; else pictureBox4.Image = zero; ed(); }
                    if (res == 4) { if (hodok == 1) pictureBox5.Image = plus; else pictureBox5.Image = zero; ed(); }
                    if (res == 5) { if (hodok == 1) pictureBox6.Image = plus; else pictureBox6.Image = zero; ed(); }
                    if (res == 6) { if (hodok == 1) pictureBox7.Image = plus; else pictureBox7.Image = zero; ed(); }
                    if (res == 7) { if (hodok == 1) pictureBox8.Image = plus; else pictureBox8.Image = zero; ed(); }
                    if (res == 8) { if (hodok == 1) pictureBox9.Image = plus; else pictureBox9.Image = zero; ed(); }
                }
                local_comp_block = false;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            int res = obrabotka_hoda(0);
            if (res == 1)
            {
                pictureBox1.Image = plus; ed(); comp_hod(1);
            }
            else if (res == 0)
            {
                pictureBox1.Image = zero; ed(); comp_hod(0);
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            int res = obrabotka_hoda(1);
            if (res == 1)
            {
                pictureBox2.Image = plus; ed(); comp_hod(1);
            }
            else if (res == 0)
            {
                pictureBox2.Image = zero; ed(); comp_hod(0);
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            int res = obrabotka_hoda(2);
            if (res == 1)
            {
                pictureBox3.Image = plus; ed(); comp_hod(1);
            }
            else if (res == 0)
            {
                pictureBox3.Image = zero; ed(); comp_hod(0);
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            int res = obrabotka_hoda(3);
            if (res == 1)
            {
                pictureBox4.Image = plus; ed(); comp_hod(1);
            }
            else if (res == 0)
            {
                pictureBox4.Image = zero; ed(); comp_hod(0);
            }
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            int res = obrabotka_hoda(4);
            if (res == 1)
            {
                pictureBox5.Image = plus; ed(); comp_hod(1);
            }
            else if (res == 0)
            {
                pictureBox5.Image = zero; ed(); comp_hod(0);
            }
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            int res = obrabotka_hoda(5);
            if (res == 1)
            {
                pictureBox6.Image = plus; ed(); comp_hod(1);
            }
            else if (res == 0)
            {
                pictureBox6.Image = zero; ed(); comp_hod(0);
            }
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            int res = obrabotka_hoda(6);
            if (res == 1)
            {
                pictureBox7.Image = plus; ed(); comp_hod(1);
            }
            else if (res == 0)
            {
                pictureBox7.Image = zero; ed(); comp_hod(0);
            }
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            int res = obrabotka_hoda(7);
            if (res == 1)
            {
                pictureBox8.Image = plus; ed(); comp_hod(1);
            }
            else if (res == 0)
            {
                pictureBox8.Image = zero; ed(); comp_hod(0);
            }
        }
        private void pictureBox9_Click(object sender, EventArgs e)
        {
            int res = obrabotka_hoda(8);
            if (res == 1)
            {
                pictureBox9.Image = plus; ed(); comp_hod(1);
            }
            else if (res == 0)
            {
                pictureBox9.Image = zero; ed(); comp_hod(0);
            }
        }

        private void toolStripStatusLabel1_TextChanged(object sender, EventArgs e)
        {
            if (now_hod_player.Text == "Ход нолика" || now_hod_player.Text == "Ожидание хода нолика" || (now_hod_player.Text == "Ваш ход" && gamer == false))
            {
                statusStrip1.BackColor = Color.Blue;
            }
            else
            {
                statusStrip1.BackColor = Color.Red;
            }
        }

        private void Main_window_FormClosing(object sender, FormClosingEventArgs e)
        { //подготовительные мероприятия по выключения приложения
            try
            {
                soc.Close();
            }
            catch(Exception ex)
            {
                new Mail_game_info().error(ex);
            }
            try
            {
                soc_mess.Close();
            }
            catch(Exception ex)
            {
                new Mail_game_info().error(ex);
            }
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 a = new AboutBox1();
            a.Show();
        }

        private void сКомпьютеромToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Хотите начать новую игру", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (res == DialogResult.OK)
            {
                hod = true; local_comp = true;
                comp_kr_or_null = -1;
                local_or_ethernet = true;
                type_game.Text = "Локальная игра";
                try
                {
                    if (soc != null)
                    {
                        soc.Close();
                        soc = null;
                    }
                }
                catch (SocketException exsoc)
                {
                    new Mail_game_info().error(exsoc);
                }
                catch (Exception ex)
                {
                    new Mail_game_info().error(ex);
                }
                try
                {
                if (soc_mess != null)
                    {
                        soc_mess.Close();
                        soc_mess = null;
                    }
                }
                catch (SocketException exsoc)
                {
                    new Mail_game_info().error(exsoc);
                }
                catch (Exception ex)
                {
                    new Mail_game_info().error(ex);
                }
                pictureBox1.Image = null;
                pictureBox2.Image = null;
                pictureBox3.Image = null;
                pictureBox4.Image = null;
                pictureBox5.Image = null;
                pictureBox6.Image = null;
                pictureBox7.Image = null;
                pictureBox8.Image = null;
                pictureBox9.Image = null;
                for (int i = 0; i < 9; i++)
                    lol[i] = 3;
            }
            else
            {
                MessageBox.Show("Новая игра отменена", "", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        private void дваИгрокаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Хотите начать новую игру", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (res == DialogResult.OK)
            {
                hod = true; local_comp = false;
                local_or_ethernet = true;
                type_game.Text = "Локальная игра";
                try
                {
                    if (soc != null)
                    {
                        soc.Close();
                        soc = null;
                    }
                }
                catch (SocketException exsoc)
                {
                    new Mail_game_info().error(exsoc);
                }
                catch (Exception ex)
                {
                    new Mail_game_info().error(ex);
                }
                try
                {
                    if (soc_mess != null)
                    {
                        soc_mess.Close();
                        soc_mess = null;
                    }
                }
                catch (SocketException exsoc)
                {
                    new Mail_game_info().error(exsoc);
                }
                catch (Exception ex)
                {
                    new Mail_game_info().error(ex);
                }
                pictureBox1.Image = null;
                pictureBox2.Image = null;
                pictureBox3.Image = null;
                pictureBox4.Image = null;
                pictureBox5.Image = null;
                pictureBox6.Image = null;
                pictureBox7.Image = null;
                pictureBox8.Image = null;
                pictureBox9.Image = null;
                for (int i = 0; i < 9; i++)
                    lol[i] = 3;
            }
            else
            {
                MessageBox.Show("Новая игра отменена", "", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        private void статистикаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Statistic().Show();
        }

        private void pic_mus_play_pause_stop_Click(object sender, EventArgs e)
        {
            if (music_play_sound)
            {
                pic_mus_play_pause_stop.Image = Resource1.pause_pic; //mus.Stop();
                music_play_sound = false;
            }
            else
            {
                pic_mus_play_pause_stop.Image = Resource1.play_pic; //mus.PlayLooping();
                music_play_sound = true;
            }
        }
    }
}