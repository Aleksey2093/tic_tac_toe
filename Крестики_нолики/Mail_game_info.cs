using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Threading;

namespace Крестики_нолики
{
    class Mail_game_info
    {
        /// <summary>
        /// Отправляет на серверй сообщение об ошибке
        /// Если сообщение об ошибке не было доставленно по причине ошибке соединения, то к сообщению добавляются сведения об обшики передачи сообщения по электронной почте.
        /// Через определенное время может будет реализовать программу и алгоритм, который будет собирать неизвестные ходы с писем эленной почты почтового ящика, 
        /// а затем помещать их в папку игрой. Для хранения такищ вещей вполне можно использовать собственный формат файла, чтобы было проще искать.
        /// Я бы предложил группирования получаемых с почты массивов ходов следующим образом. Во-первых разделить на два файла. 
        /// Во-вторых разгрупировать их по столбцам. Таким образом мы будем сразу отсеивать несовпадающие элементы не делая лишних движений.
        /// Соответственно эту штука окажеться очень полезным инструментом для следующих игр
        /// </summary>
        /// <param name="err">Сообщение об ошибке в страндартном формате Exception</param>
        public void error(Exception err)
        {
            new Thread(delegate ()
            { 
                int i = 0; Exception exx = null;
            restart:
                try
                {
                SmtpClient smtp = new SmtpClient("smtp.yandex.ru", 25);
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential("gamealeksey2093", "+++gamealeksey2093+++");
                MailMessage mail_message = new MailMessage();
                mail_message.From = new MailAddress("gamealeksey2093@yandex.ru", "gamer_tic_tac_toe");
                mail_message.To.Add(new MailAddress("gamealeksey2093@ya.ru"));
                mail_message.Subject = "Ошибка в игре " + DateTime.Now.ToString();
                mail_message.Body = err.ToString() + "\n \n" + DateTime.Now.ToString() + "\n \n" + exx.ToString();
                    smtp.Send(mail_message);
                }
                catch (Exception ex)
                {
                    i++;
                    if (i < 5) { exx = ex; goto restart; }
                }
            }).Start();
        }
        /// <summary>
        /// Отправка на сервер информации о неудачной игре. 
        /// Информация передается в виде: сначала указание на то за кого играет компьютер (крестик или нолик) далее через пробел перечисляются ходы совершенные игроком и компьютером за время игры.
        /// Первым всегда ходит крестик, поэтому указание на то кто ходит первым не передается.
        /// При необходимости данная функция может дополняться.
        /// </summary>
        /// <param name="hody">строковая переменная, которая хранит информацию о совершенных ходах</param>
        public void info_send(string hody) //сообщение о проигрыше компьютера
        {
            new Thread(delegate ()
           {
               int i = 0; Exception exx = null;
           restart:
               SmtpClient smtp = new SmtpClient("smtp.yandex.ru", 25);
               smtp.EnableSsl = true;
               smtp.Credentials = new NetworkCredential("gamealeksey2093", "+++gamealeksey2093+++");
               MailMessage mail_message = new MailMessage();
               mail_message.From = new MailAddress("gamealeksey2093@yandex.ru", "gamer_tic_tac_toe");
               mail_message.To.Add(new MailAddress("gamealeksey2093@ya.ru"));
               mail_message.Subject = "Ходы игры в которой компьютер проиграл";
               mail_message.Body = hody + "\n \n" + DateTime.Now.ToString();
               try
               {
                   smtp.Send(mail_message);
               }
               catch (Exception ex)
               {
                   i++;
                   if (i < 5) { exx = ex; goto restart; }
               }
           }).Start();
        }
    }
}
