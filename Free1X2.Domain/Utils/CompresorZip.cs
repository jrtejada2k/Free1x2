// Free1X2 · WinUI 3 — WIN3
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2008 Morrison - morrison [dot] ne [at] gmail [dot] com
// (Basado en el Compresor *.z2q de Charlie)
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// -----------------------------------------------------------------------------
// Portado a Free1X2.Domain (migración WinUI 3) desde Free1X2/Utils/CompresorZip.cs.
// Mantiene 1:1 el formato de archivo *.z3q del WinForms original: un archivo ZIP
// (ICSharpCode.SharpZipLib) con una única entrada "Deflated" cuyos bytes empaquetan
// el BitArray de columnas (bit i -> byte[i/8], LSB primero: 0x01 << (i%8)).
// No se cambia el formato; los *.z3q existentes siguen siendo compatibles.
// Única diferencia respecto al legacy: Crc32 pasó al namespace
// ICSharpCode.SharpZipLib.Checksum en SharpZipLib 1.4.x.
// -----------------------------------------------------------------------------

using System.Collections;
using System.IO;
using ICSharpCode.SharpZipLib.Checksum;
using ICSharpCode.SharpZipLib.Zip;

namespace Free1X2.Utils
{
    public class CompresorZip
    {
        readonly FastZip fzip = new FastZip();
        private const int BUFFER = 2048;

        public static long Comprimir(BitArray bs, string fichZip, string fichBin, int tamano, int nivel)
        {
            ZipOutputStream outZip = new ZipOutputStream(File.Create(fichZip));

            byte[] matrizBits = new byte[(tamano / 8) + 1];

            int count = 0;
            for (int i = 0; i < matrizBits.Length; i++)
            {
                for (int bit = 0; bit < 8; bit++)
                {
                    try
                    {
                        if (bs[count++])
                        {
                            matrizBits[i] |= (byte)(0x01 << bit);
                        }
                    }
                    catch
                    {
                    }
                }
            }
            var crc = new Crc32();
            crc.Update(matrizBits); // Calculate CRC for the data

            ZipEntry entry = new ZipEntry(fichBin); // ?????? Cambiar Nombre del fichero con extension .bin y sin path

            entry.CompressionMethod = CompressionMethod.Deflated;

            entry.Size = matrizBits.Length;
            // CRC
            entry.Crc = crc.Value;

            outZip.SetLevel(nivel);
            outZip.PutNextEntry(entry);
            outZip.Write(matrizBits, 0, matrizBits.Length);
            outZip.CloseEntry();

            long outSize = entry.Size;
            outZip.Flush();
            outZip.Close();

            return outSize;
        }


        public static BitArray Descomprimir(string fichComprimido)
        {
            BitArray bs = new BitArray(14348907);

            int count = 0;

            ZipInputStream inputZip = new ZipInputStream(File.OpenRead(fichComprimido));

            ZipEntry entry = inputZip.GetNextEntry();

            // extract file if not a directory
            if (!entry.IsDirectory)
            {

                int numBytes;
                byte[] data = new byte[BUFFER];
                while ((numBytes = inputZip.Read(data, 0, BUFFER)) > 0)
                {
                    for (int i = 0; i < numBytes; i++)
                    {
                        byte car = data[i];
                        for (int p = 0; p < 8; p++)
                        {
                            if (((car & 0xFF) & (0x01 << p)) != 0)
                            {
                                int numero = count + i * 8 + p;
                                bs.Set(numero, true);
                            }
                        }
                    }
                    count += numBytes * 8;
                }
                inputZip.Close();
            }

            int tamanoReal = 4782969;
            if (count > 14348907)
            {
                tamanoReal = 14348907;
            }


            BitArray bsSal = new BitArray(tamanoReal);
            for (int i = 0; i < tamanoReal; i++)
            {
                bsSal.Set(i, bs.Get(i));
            }
            return bsSal;
        }


        public void CreateZip(string zipFileName, string sourceDirectory, bool recurse, string fileFilter)
        {
            fzip.CreateZip(zipFileName, sourceDirectory, recurse, fileFilter);
        }

        public void ExtractZip(string zipFileName, string targetDirectory, string fileFilter)
        {
            fzip.ExtractZip(zipFileName, targetDirectory, fileFilter);
        }
    }
}
