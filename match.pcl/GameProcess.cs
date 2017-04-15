using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;


namespace match
{
	// класс "Игровой процесс"
	class GameProcess
    {
        private static GameProcess instance = new GameProcess ();
        public GameTime gameTime = new GameTime ();

        public bool mainMenu;
        public bool IsGame;
        public bool scoreMenu;
        public float startedTime = 0;

        public static List<int> Score = new List<int> (){ 0, 0, 0, 0, 0, 0 };
        public static List<List<Element>> ListElements = new List<List<Element>> ();
        public static List<Boulder> Boulders = new List<Boulder> ();

        public static int TotalScore {
            get {
                int _ = 0;
                foreach (int i in Score)
                    _ += i;
                return _;
            }
        }

        public GameProcess ()
        {
            mainMenu = true; // поражения не было
            IsGame = false; // игровой процесс еще не идет
            IsGame = false; // пауза не включена

        }
        // установка новой игры (подразумевает отрисовку начального экрана)
        public void NewGame ()
        {
            var count = 0;

            while (true) {
                count++;
                for (var i = 0; i < 8; i++) {
                    var list = new List<Element> ();
                    for (var j = 0; j < 8; j++) {
                        list.Add (new Element (i, j, instance));
                    }
                    ListElements.Add (list); 
                }
                if (!Element.CheckPossibilityToDestroy ()) {
                    startedTime += (float)gameTime.ElapsedGameTime.TotalSeconds; //Time passed since last Update() 
                    break;
                }
                Debug.WriteLine (count + " ");
                ListElements = new List<List<Element>> ();
            }

            mainMenu = false;
            IsGame = true;     
        }
        // установка победы
        public void WinGame ()
        {
            mainMenu = false;
            IsGame = false;
        }
        // установка поражения
        public void FinishGame ()
        {
            mainMenu = false;
            IsGame = false;
            scoreMenu = true;
        }

        public void Restart ()
        {
            mainMenu = true;
            IsGame = false;
            scoreMenu = false;


            Score = new List<int> (){ 0, 0, 0, 0, 0, 0 };
            ListElements = new List<List<Element>> ();
            Boulders = new List<Boulder> ();
            startedTime = 0;

        }
    }
}

