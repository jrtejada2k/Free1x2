// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;

namespace Free1X2.Escrutinio
{
	/// <summary>
	/// Summary description for BitacoraComparer.
	/// </summary>
	public class PosiblesPremiosComparer : IComparer<PosiblesPremiosContenedor>
	{
        public int Compare(PosiblesPremiosContenedor x, PosiblesPremiosContenedor y)
		{
	        if(x == null || y == null)
				throw new ArgumentException("Object does not have a Name type.");

			PosiblesPremiosContenedor object1 = x;
			PosiblesPremiosContenedor object2 = y;

			int valor = object2.Col16.Count.CompareTo(object1.Col16.Count);

			if(valor == 0)
			{	
				valor = object2.Col15.Count.CompareTo(object1.Col15.Count);
					if(valor == 0)
					{
						valor = object2.Col14.Count.CompareTo(object1.Col14.Count);
							if(valor == 0)
							{
								valor = object2.Col13.Count.CompareTo(object1.Col13.Count);
									if(valor == 0)
									{
										valor = object2.Col12.Count.CompareTo(object1.Col12.Count);
                                        if (valor == 0)
                                        {
                                            valor = object2.Col11.Count.CompareTo(object1.Col11.Count);
                                            if (valor == 0)
                                            {
                                                valor = object2.Col10.Count.CompareTo(object1.Col10.Count);
                                            }
                                        }
									}
							}
					}
			}
			return valor;
		}
	}
}
