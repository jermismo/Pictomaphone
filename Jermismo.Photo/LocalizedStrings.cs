using System;
using System.Net;
using System.Windows;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Jermismo.Photo
{
    public class LocalizedStrings
    {

        public LocalizedStrings() { }

        private static Jermismo.Photo.Resources.AppResources localizedResources = new Resources.AppResources();

        /// <summary>
        /// Gets the list of localized resources.
        /// </summary>
        public Jermismo.Photo.Resources.AppResources LocalizedResources { get { return localizedResources; } }

        // These strings are internal and not in a resource, because they handle system
        // level exception messages. (the type you might get if the resources were missing)
        // Why the extra safety? I guess I'm paranoid. Having your error message error out sucks!

        internal static string GetError(CultureInfo cultureInfo)
        {
            switch (cultureInfo.TwoLetterISOLanguageName)
            {
                case "de":   // german
                    return "Fehler";
                case "nl":   // dutch
                    return "Fout";
                case "fr":   // french
                    return "Erreur";
                case "it":   // italian
                    return "Errore";
                case "pl":   // polish
                    return "Błąd";
                case "zh":   // chinese
                    return "错误";
                case "ja":   // japanese
                    return "エラー";
                case "pt":   // portugese
                    return "Erro";
                case "ru":   // russian
                    return "Ошибка";
                case "sv":   // swedish
                    return "Fel";
                case "cs":   // czech
                    return "Chyba";
                case "es":   // spanish
                case "en":   // english
                default:
                    return "Error";
            }
        }

        internal static string GetSystemErrorMessage(CultureInfo cultureInfo)
        {
            switch (cultureInfo.TwoLetterISOLanguageName)
            { 
                case "de":   // german
                    return "Etwas Schlimmes ist passiert. Möchten Sie den Fehler senden e-Mail an mich?";
                case "nl":   // dutch
                    return "Slecht iets is gebeurd. Wilt u e-mail de fout naar me?";
                case "fr":   // french
                    return "Quelque chose de mal est arrivé. Voulez vous m'envoyer l'erreur ?";
                case "es":   // spanish
                    return "Algo malo ocurrió. ¿Desea enviar el error a mí?";
                case "it":   // italian
                    return "Qualcosa di brutto è accaduto. Vuoi mandare l'errore e-mail a me?";
                case "pl":   // polish
                    return "Zły coś się stało. Czy chcesz wysłać ten błąd do mnie?";
                case "zh":   // chinese
                    return "不好的事情发生了。想要给我发邮件错误吗？";
                case "ja":   // japanese
                    return "悪い何かが起こった。エラーを私にメールしたいですか？";
                case "pt":   // portugese
                    return "Algo de ruim aconteceu. Deseja enviar e-mail o erro para mim?";
                case "ru":   // russian
                    return "Случилось что-то плохое. Хотите по электронной почте об ошибке?";
                case "sv":   // swedish
                    return "Hände något dåligt. Vill du e-posta felet till mig?";
                case "cs":   // czech
                    return "Stalo se něco ošklivého. Chcete odeslat tuto chybu mi e-mailem?";
                case "en":   // english
                default:
                    return "Something bad happened. Want to email the error to me?";
            }
        }

    }
}
