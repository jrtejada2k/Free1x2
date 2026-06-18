namespace Free1X2.Analisis
{
    public class ContenedorDiferencias
    {
        //Debe contener toda la información de una Repetición
        private int[] var, equis, doses, dibujos, interrupciones, formatos;
        private int noRep;
        public ContenedorDiferencias(int numDiferenciasPosibles)
        {
            Variantes = new int[numDiferenciasPosibles + 1];
            Equis = new int[numDiferenciasPosibles + 1];
            Doses = new int[numDiferenciasPosibles + 1];
            Dibujos = new int[numDiferenciasPosibles + 1];
            Interrupciones = new int[numDiferenciasPosibles + 1];
            Formatos = new int[numDiferenciasPosibles + 1];
            noRep = numDiferenciasPosibles + 1;
        }

        public int[] Variantes
        {
            get { return var; }
            set { var = value; }
        }
        public int[] Equis
        {
            get { return equis; }
            set { equis = value; }
        }
        public int[] Doses
        {
            get { return doses; }
            set { doses = value; }
        }
        public int[] Dibujos
        {
            get { return dibujos; }
            set { dibujos = value; }
        }
        public int[] Interrupciones
        {
            get { return interrupciones; }
            set { interrupciones = value; }
        }
        public int[] Formatos
        {
            get { return formatos; }
            set { formatos = value; }
        }
        public int NumDiferenciasPosibles
        {
            get { return noRep; }
        }
    }
}
