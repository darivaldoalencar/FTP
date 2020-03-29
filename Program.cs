using System;
using System.IO;
using System.Net;
using System.Xml;

namespace FTPDownload
{
    class Program
    {
        #region propriedados
        private string pathRaiz = "C:/ProjetosWeb/TesteFTP/FTP";
        private string fileTMP = "/FTPConfig.xml";        
        string urlFTP;
        string usuario;
        string senha;
        #endregion

        #region estrutura do XML lido
        //<FTP> 
        //  <user>usuarioftp</user>
        //  <pas>senhaftp</pas>   
        //  <url>ftp://hostcompleto/</url>
        //</FTP>		
        //OBS: tem uma barra no final da url
        #endregion

        #region métodos
        private bool LerXMLConfig()
        {
            Console.WriteLine("Lendo arquivo XML de configuração.");
            if (File.Exists(string.Concat(pathRaiz, this.fileTMP)))
            {
                XmlDocument oXML = new XmlDocument();
                oXML.Load(string.Concat(pathRaiz, this.fileTMP));
                this.usuario = oXML.SelectSingleNode("FTP").SelectSingleNode("user").InnerText;
                this.senha = oXML.SelectSingleNode("FTP").SelectSingleNode("pas").InnerText;
                this.urlFTP = oXML.SelectSingleNode("FTP").SelectSingleNode("url").InnerText;
                return true;
            }
            else
            {
                Console.WriteLine("O XML de configuração gerado pelo Delphi não foi localizado!");
                return false;
            }
        }

        private void FazDownlod(string nomeArquivo)
        {
            Program p = new Program();
            if (!p.LerXMLConfig())
                return;

            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(string.Concat(p.urlFTP, nomeArquivo));
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                //request.KeepAlive = true;
                //request.UsePassive = true;
                //request.UseBinary = true;
                request.Credentials = new NetworkCredential(p.usuario, p.senha);

                Console.WriteLine("conectado FTP.");
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    using (Stream rs = response.GetResponseStream())
                    {
                        Console.WriteLine("Fazendo download do arquivo.");
                        using (FileStream ws = new FileStream(string.Concat(p.pathRaiz, nomeArquivo), FileMode.Create))
                        {
                            byte[] buffer = new byte[2048];
                            int bytesRead = rs.Read(buffer, 0, buffer.Length);
                            while (bytesRead > 0)
                            {
                                ws.Write(buffer, 0, bytesRead);
                                bytesRead = rs.Read(buffer, 0, buffer.Length);
                            }
                        }
                    }
                    Console.WriteLine(string.Concat("Download concluido em: ", p.pathRaiz, nomeArquivo));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro: " + ex.Message);
            }

        }
        #endregion

        static void Main(string[] args)
        {
            Program p = new Program();
            p.FazDownlod("/VersaoFTP.INI");
        }
    }
}
