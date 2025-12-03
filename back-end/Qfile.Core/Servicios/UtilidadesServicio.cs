using System;
using System.Collections.Generic;
using System.Text;

namespace Qfile.Core.Servicios
{
    public static class UtilidadesServicio
    {
        public static DateTime FechaActualServidor {
            get { return DateTime.Now; }
        }

        public static DateTime FechaActualUtc {
            get { return DateTime.UtcNow; }
        }

        public static DateTime FechaActualCliente(string timeZone = null)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZone));
        }

        public static string GenerarCadenaAleatoria()
        {
            var caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!#$%&/()";
            var stringCaracteres = new char[8];
            var random = new Random();

            for (int i = 0; i < stringCaracteres.Length; i++)
            {
                stringCaracteres[i] = caracteres[random.Next(caracteres.Length)];
            }

            return new String(stringCaracteres);
        }

        public static string UrlQfile
        {
            get
            {
                return "http://localhost:4200/";
            }
        }
    }
}

/*
===============================================================================================
Lista de Time Zone Ids
===============================================================================================

Dateline Standard Time

UTC-11

Samoa Standard Time

Hawaiian Standard Time

Alaskan Standard Time

Pacific Standard Time (Mexico)

Pacific Standard Time

US Mountain Standard Time

Mountain Standard Time (Mexico)

Mountain Standard Time

Central America Standard Time

Central Standard Time

Central Standard Time (Mexico)

Canada Central Standard Time

SA Pacific Standard Time

Eastern Standard Time

US Eastern Standard Time

Venezuela Standard Time

Paraguay Standard Time

Atlantic Standard Time

Central Brazilian Standard Time

SA Western Standard Time

Pacific SA Standard Time

Newfoundland Standard Time

E. South America Standard Time

Argentina Standard Time

SA Eastern Standard Time

Greenland Standard Time

Montevideo Standard Time

UTC-02

Mid-Atlantic Standard Time

Azores Standard Time

Cape Verde Standard Time

Morocco Standard Time

UTC

GMT Standard Time

Greenwich Standard Time

W. Europe Standard Time

Central Europe Standard Time

Romance Standard Time

Central European Standard Time

W. Central Africa Standard Time

Namibia Standard Time

Jordan Standard Time

GTB Standard Time

Middle East Standard Time

Egypt Standard Time

Syria Standard Time

South Africa Standard Time

FLE Standard Time

Israel Standard Time

E. Europe Standard Time

Arabic Standard Time

Arab Standard Time

Russian Standard Time

E. Africa Standard Time

Iran Standard Time

Arabian Standard Time

Azerbaijan Standard Time

Mauritius Standard Time

Georgian Standard Time

Caucasus Standard Time

Afghanistan Standard Time

Ekaterinburg Standard Time

Pakistan Standard Time

West Asia Standard Time

India Standard Time

Sri Lanka Standard Time

Nepal Standard Time

Central Asia Standard Time

Bangladesh Standard Time

N. Central Asia Standard Time

Myanmar Standard Time

SE Asia Standard Time

North Asia Standard Time

China Standard Time

North Asia East Standard Time

Singapore Standard Time

W. Australia Standard Time

Taipei Standard Time

Ulaanbaatar Standard Time

Tokyo Standard Time

Korea Standard Time

Yakutsk Standard Time

Cen. Australia Standard Time

AUS Central Standard Time

E. Australia Standard Time

AUS Eastern Standard Time

West Pacific Standard Time

Tasmania Standard Time

Vladivostok Standard Time

Central Pacific Standard Time

New Zealand Standard Time

UTC+12

Fiji Standard Time

Kamchatka Standard Time

Tonga Standard Time
     
*/
