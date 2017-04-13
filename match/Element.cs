using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace match
{
    // class for main element of the game
    class Element
    {
        private static float SPEED_ROTATION = 0.01f * Game1.GLOBAL_SPEED;
        private static float SPEED_SELF_ROTATION = 0.005f * Game1.GLOBAL_SPEED;
        private static float SPEED_FALLING = 0.003f * Game1.GLOBAL_SPEED;

        public GameProcess GameProcess = new GameProcess ();

        public int X { get; private set; }

        public int Y { get; private set; }

        public float Rate { get; set; }

        public float Opacity { get; set; }

        public int TypeOf { get; private set; }

        public int Bonus { get; set; }

        public static bool IsSelected { get; set; }


        private static Element _selected;

        private static Element ElementUnderMouse { get; set; }

        private static List<Element> AnimatedByMouse = new List<Element> ();

        public class Point
        {
            public float xCoord;
            public float yCoord;

            public Point (float x, float y)
            {
                xCoord = x;
                yCoord = y;
            }
        };

        private static Random rnd = new Random (0);

        private bool IsBlocked;
        private Point rotationCenter;
        private Point swapWith;
        private Element swapWithElement;
        private int timeStartRotation;
        private bool rotateClockwise;
        private bool bonusDirection;
        // true - horisontal; false - vertical

        private long timeStartFalling;
        private int falling = 0;

        private bool isSwapValid = true;
        private bool haveToBeDestroyed = false;
        private bool isDestoyed = false;
        private int timeStartSelfRotation = 0;

        public float XTextureRotationMultiplier {
            get {
                float A = (float)Math.Cos (Math.PI / 4);
                return (float)(A - Math.Cos (TextureRotationAngle () - Math.PI / 4)) / (2 * A);
            }
        }

        public float YTextureRotationMultiplier {
            get {
                float A = (float)Math.Sin (Math.PI / 4);
                return (float)(A + Math.Sin (TextureRotationAngle () - Math.PI / 4)) / (2 * A);
            }
        }

        public float TextureRotationAngle ()
        {
            if (!haveToBeDestroyed)
                return 0;
            if (timeStartSelfRotation == 0)
                timeStartSelfRotation = Environment.TickCount;
            Rate -= 0.001f;
            return (Environment.TickCount - timeStartSelfRotation) * SPEED_SELF_ROTATION;
        }

        
        static Dictionary<int, int> newElementsHaveToAdd = new Dictionary<int, int> ()
        { { 0,0 }, { 1,0 }, { 2,0 }, { 3,0 }, { 4,0 }, { 5,0 }, { 6,0 }, { 7,0 } };
        static Dictionary<int, bool> newElementsWasAdded = new Dictionary<int, bool> ()
        { { 0,false }, { 1,false }, { 2,false }, { 3,false }, { 4,false }, { 5,false }, { 6,false }, { 7,false } };

        public Element (int index_x, int index_y, GameProcess importGameProcess, 
                        int elem_type = -1, int elem_bonus = -1, bool dir = false)
        {
            X = index_x;
            Y = index_y;
            if (elem_type == -1) {
                TypeOf = rnd.Next (0, 6);
                Rate = 1;
                Opacity = 1;
                Bonus = 0;
            } else {
                Rate = 0;
                Opacity = 1;
                TypeOf = elem_type;
                Bonus = elem_bonus;
                bonusDirection = dir;
            }

            IsBlocked = false;
            GameProcess = importGameProcess;

           
        }

        public static bool CheckPossibilityToDestroy (Element e = null)
        {
            if (e != null) {
                int hms = e.X;
                int hme = e.X;
                while (hms - 1 >= 0 &&
                       e.TypeOf == GameProcess.ListElements [hms - 1] [e.Y].TypeOf &&
                       GameProcess.ListElements [hms - 1] [e.Y].falling == 0 &&
                       !GameProcess.ListElements [hms - 1] [e.Y].isDestoyed) {
                    hms--;
                }
                while (hme + 1 < 8 &&
                       e.TypeOf == GameProcess.ListElements [hme + 1] [e.Y].TypeOf &&
                       GameProcess.ListElements [hme + 1] [e.Y].falling == 0 &&
                       !GameProcess.ListElements [hme + 1] [e.Y].isDestoyed) {
                    hme++;
                }
                if (1 + hme - hms >= 3)
                    return true;
                int vms = e.Y;
                int vme = e.Y;
                while (vms - 1 >= 0 &&
                       e.TypeOf == GameProcess.ListElements [e.X] [vms - 1].TypeOf &&
                       GameProcess.ListElements [e.X] [vms - 1].falling == 0 &&
                       !GameProcess.ListElements [e.X] [vms - 1].isDestoyed) {
                    vms--;
                }
                while (vme + 1 < 8 &&
                       e.TypeOf == GameProcess.ListElements [e.X] [vme + 1].TypeOf &&
                       GameProcess.ListElements [e.X] [vme + 1].falling == 0 &&
                       !GameProcess.ListElements [e.X] [vme + 1].isDestoyed) {
                    vme++;
                }
                if (1 + vme - vms >= 3)
                    return true;
                return false;
            } else {
                foreach (var lst in GameProcess.ListElements) {
                    foreach (var elem in lst) {
                        if (CheckPossibilityToDestroy (elem))
                            return true;
                    }
                }
                return false;
            }

        }

        static void DestroySelectedElement (Element e)
        {
            List <Element> toDestroy = new List<Element> ();
            int hms = e.X;
            int hme = e.X;
            while (hms - 1 >= 0 && e.TypeOf == GameProcess.ListElements [hms - 1] [e.Y].TypeOf &&
                   !GameProcess.ListElements [hms - 1] [e.Y].isDestoyed &&
                   GameProcess.ListElements [hms - 1] [e.Y].falling == 0) {
                hms--;
            }
            while (hme + 1 < 8 && e.TypeOf == GameProcess.ListElements [hme + 1] [e.Y].TypeOf &&
                   !GameProcess.ListElements [hme + 1] [e.Y].isDestoyed &&
                   GameProcess.ListElements [hme + 1] [e.Y].falling == 0) {
                hme++;
            }
            bool hm = (1 + hme - hms >= 3);
            int vms = e.Y;
            int vme = e.Y;
            while (vms - 1 >= 0 && e.TypeOf == GameProcess.ListElements [e.X] [vms - 1].TypeOf &&
                   !GameProcess.ListElements [e.X] [vms - 1].isDestoyed &&
                   GameProcess.ListElements [e.X] [vms - 1].falling == 0) {
                vms--;
            }
            while (vme + 1 < 8 && e.TypeOf == GameProcess.ListElements [e.X] [vme + 1].TypeOf &&
                   !GameProcess.ListElements [e.X] [vme + 1].isDestoyed &&
                   GameProcess.ListElements [e.X] [vme + 1].falling == 0) {
                vme++;
            }
            bool vm = (1 + vme - vms >= 3);

            if (hm) {
                for (int i = hms; i <= hme; i++) {
                    toDestroy.Add (GameProcess.ListElements [i] [e.Y]);
                }
            }
            if (vm) {
                for (int i = vms; i <= vme; i++) {
                    toDestroy.Add (GameProcess.ListElements [e.X] [i]);
                }
            }
            toDestroy = toDestroy.Distinct ().ToList ();
            if (toDestroy.Count > 3) {
                Element q;
                if (toDestroy.Capacity == 4)
                    q = new Element (e.X, e.Y, e.GameProcess, e.TypeOf, 1, 
                        (hme - hms > vme - vms) ? true : false);
                else
                    q = new Element (e.X, e.Y, e.GameProcess, e.TypeOf, 2);
                GameProcess.ListElements [e.X] [e.Y] = q;
                AnimatedByMouse.Add (GameProcess.ListElements [e.X] [e.Y]);
                toDestroy.Remove (e);
            }
            foreach (var a in toDestroy) {
                destroyExactElement (a);
            }

        }

        public static void destroyExactElement (Element a)
        {
            if (a.IsBlocked || a.falling != 0 || a.isDestoyed)
                return;
            a.Opacity = 0;
            a.isDestoyed = true;
            GameProcess.Score [a.TypeOf] += 1;
            if (a.Bonus == 1) {
                GameProcess.Boulders.Add (new Boulder (a.getXcoord (), a.getYcoord (), (a.bonusDirection) ? 0 : 1));
                GameProcess.Boulders.Add (new Boulder (a.getXcoord (), a.getYcoord (), (a.bonusDirection) ? 2 : 3));
            }
            if (a.Bonus == 2) {
                for (int i = -1; i <= 1; i++)
                    for (int j = -1; j <= 1; j++)
                        if (a.X + i > 0 && a.X + i < 8 && a.Y + j > 0 && a.Y + j < 8)
                            GameProcess.ListElements [a.X + i] [a.Y + j].haveToBeDestroyed = true;
            }

            if (a.Y == 0) {
                GameProcess.ListElements [a.X] [0] = new Element (a.X, 0, a.GameProcess);
                GameProcess.ListElements [a.X] [0].Rate = 0;
                AnimatedByMouse.Add (GameProcess.ListElements [a.X] [0]);
                DestroySelectedElement (GameProcess.ListElements [a.X] [0]);
            } else
                newElementsHaveToAdd [a.X]++;
            for (int i = a.Y - 1; i >= 0; i--) {
                GameProcess.ListElements [a.X] [i].falling++;
                if (GameProcess.ListElements [a.X] [i].timeStartFalling == 0)
                    GameProcess.ListElements [a.X] [i].timeStartFalling = Environment.TickCount;
            }
        }

        int fallingCalc ()
        {
            float y = Game1.INDENT + Game1.WIDTH * Y;
            float dy = (Environment.TickCount - timeStartFalling) * SPEED_FALLING;
            Element e = this;

            if (dy > 0.5f && dy < 0.9f && !newElementsWasAdded [X]) {
                
                int i = e.Y;
                while (i < 8 && GameProcess.ListElements [e.X] [i].falling > 0)
                    i++;
                for (int j = i == 8 ? 7 : i; j > 0; j--)
                    GameProcess.ListElements [e.X] [j] = GameProcess.ListElements [e.X] [j - 1];
                
                GameProcess.ListElements [e.X] [0] = new Element (e.X, 0, GameProcess);
                GameProcess.ListElements [e.X] [0].Rate = 0;
                AnimatedByMouse.Add (GameProcess.ListElements [e.X] [0]);
                newElementsWasAdded [X] = true;
            } else if (dy >= 1) {
                Y++;
                falling--;
                if (Y == 1) {
                    GameProcess.ListElements [X] [0].falling = falling;
                    if (falling > 0)
                        GameProcess.ListElements [X] [0].timeStartFalling = Environment.TickCount;
                    else
                        DestroySelectedElement (GameProcess.ListElements [X] [0]);
                }

                if (falling != 0)
                    timeStartFalling = Environment.TickCount;
                else {
                    timeStartFalling = 0;
                    DestroySelectedElement (this);
                }
                newElementsWasAdded [X] = false;
            }
            return (int)Math.Round (y + Game1.WIDTH * dy);
        }

        public int getXcoord ()
        {
            float x = Game1.INDENT + Game1.WIDTH * X;
            if (rotationCenter != null) {
                return rotationHandlerByX ();
            }
            this.checkMe ();
            return (int)Math.Round (x);
        }

        private void checkMe ()
        {
            if (haveToBeDestroyed)
                destroyExactElement (this);
            if (this != _selected)
                IsBlocked = false;
            if (Y == 6 && falling == 0 && GameProcess.ListElements [X] [7].isDestoyed)
                falling++;
        }

        public int getYcoord ()
        {
            float y = Game1.INDENT + Game1.WIDTH * Y;
            if (this.rotationCenter != null) {
                return rotationHandlerByY ();
            }   
            if (falling != 0)
                return fallingCalc ();

            return (int)Math.Round (y);
        }

        public int rotationHandlerByY ()
        {
            float x = Game1.INDENT + Game1.WIDTH * X;
            float y = Game1.INDENT + Game1.WIDTH * Y;

            float dphi = Math.Min ((Environment.TickCount - this.timeStartRotation) * SPEED_ROTATION, (float)Math.PI);
            // (Environment.TickCount - this.timeStartRotation) * SPEED_ROTATION;
            float rad = (float)Math.Sqrt (Math.Pow (x - this.rotationCenter.xCoord, 2) +
                        Math.Pow (y - this.rotationCenter.yCoord, 2));
            float phi_0 = (float)Math.Atan2 (y - this.rotationCenter.yCoord, x - this.rotationCenter.xCoord);
            y = this.rotationCenter.yCoord + rad * (float)Math.Sin (phi_0 + (rotateClockwise ? -1 : 1) * dphi);

            return (int)Math.Round (y);

        }

        public int rotationHandlerByX ()
        {
            float x = Game1.INDENT + Game1.WIDTH * X;
            float y = Game1.INDENT + Game1.WIDTH * Y;
            float dphi = Math.Min ((Environment.TickCount - this.timeStartRotation) * SPEED_ROTATION, (float)Math.PI);
            //                    (Environment.TickCount - this.timeStartRotation) * SPEED_ROTATION;
            float rad = (float)Math.Sqrt (Math.Pow (x - this.rotationCenter.xCoord, 2) +
                        Math.Pow (y - this.rotationCenter.yCoord, 2));
            float phi_0 = (float)Math.Atan2 (y - this.rotationCenter.yCoord, x - this.rotationCenter.xCoord);
            x = this.rotationCenter.xCoord + rad * (float)Math.Cos (phi_0 + (rotateClockwise ? -1 : 1) * dphi);
            if (dphi == (float)Math.PI) {
                this.IsBlocked = false;
                this.swapWithElement.IsBlocked = false;
                this.rotationCenter = null;
                this.swapWithElement.rotationCenter = null;
                this.X = (int)Math.Round (this.swapWith.xCoord);
                this.Y = (int)Math.Round (this.swapWith.yCoord);
                this.swapWithElement.X = (int)Math.Round (this.swapWithElement.swapWith.xCoord);
                this.swapWithElement.Y = (int)Math.Round (this.swapWithElement.swapWith.yCoord);
                if (CheckPossibilityToDestroy (this) || CheckPossibilityToDestroy (this.swapWithElement) || !this.isSwapValid) {
                    Console.WriteLine ("Valid swap, gj");
                    this.isSwapValid = true;
                    this.swapWithElement.isSwapValid = true;
                    DestroySelectedElement (this);
                    DestroySelectedElement (this.swapWithElement);

                } else {
                    this.isSwapValid = false;
                    this.IsBlocked = false; 
                    this.swapWithElement.isSwapValid = false;
                    StartSwapAnimation (this.swapWithElement, this);
                }

                _selected = null;
            }
            return (int)Math.Round (x);
        }


        public static void markToDestroy (int x, int y)
        {
            int xx = x - Game1.INDENT;
            int yy = y - Game1.INDENT;
            Element e;
            if (xx >= Game1.GAMEFIELD || yy >= Game1.GAMEFIELD || xx < 0 || yy < 0) {
                e = null;
            } else {
                try {
                    e = GameProcess.ListElements [xx / Game1.WIDTH] [yy / Game1.WIDTH];
                } catch (NullReferenceException) {
                    return;
                }
                e.haveToBeDestroyed = true;
            }
        }

        static void StartSwapAnimation (Element first, Element clicked)
        {

            float x = (first.getXcoord () + clicked.getXcoord ()) * 0.5f;
            float y = (first.getYcoord () + clicked.getYcoord ()) * 0.5f;
            first.rotateClockwise = (first.getXcoord () < clicked.getXcoord ()
            || first.getYcoord () < clicked.getYcoord ());
            Console.WriteLine (String.Format ("Selected point coords:{0} {1}", first.getXcoord (), first.getYcoord ()));
            Console.WriteLine (String.Format ("Selected point coords:{0} {1}", clicked.getXcoord (), clicked.getYcoord ()));
            clicked.rotateClockwise = first.rotateClockwise;
            first.rotationCenter = new Point (x, y);
            clicked.rotationCenter = first.rotationCenter;

            Console.WriteLine (String.Format ("Center of rotation is {0} {1}", first.rotationCenter.xCoord, first.rotationCenter.yCoord));


            first.timeStartRotation = Environment.TickCount;
            clicked.timeStartRotation = first.timeStartRotation;
            AnimatedByMouse.Add (clicked);
            AnimatedByMouse.Add (first);

            GameProcess.ListElements [first.X] [first.Y] = clicked;
            GameProcess.ListElements [clicked.X] [clicked.Y] = first;

            clicked.swapWith = new Point (first.X, first.Y);
            first.swapWith = new Point (clicked.X, clicked.Y);
            clicked.swapWithElement = first;
            first.swapWithElement = clicked;
            clicked.IsBlocked = true;
            first.IsBlocked = true;
            first = null;
        }

        public static void CursorClickHandler (Vector2 cursorPos)
        {
            
            int xx = (int)cursorPos.X - Game1.INDENT;
            int yy = (int)cursorPos.Y - Game1.INDENT;
            var clicked = GameProcess.ListElements [xx / Game1.WIDTH] [yy / Game1.WIDTH];

            if (clicked.IsBlocked || clicked.isDestoyed || clicked.falling > 0)
                return;
            KeyboardState state = Keyboard.GetState ();
            if (state.IsKeyDown (Keys.LeftShift)) {
                clicked.TypeOf = rnd.Next (0, 6);
                return;
            }

            if (_selected != null && clicked.IsAdjacentToSelectedElement () && _selected.falling == 0) {
                StartSwapAnimation (_selected, clicked);
            } else if (_selected == clicked) {
                _selected.IsBlocked = false;
                _selected = null;

            } else {
                if (_selected != null)
                    _selected.IsBlocked = false;
                _selected = clicked;
                _selected.IsBlocked = true;
                AnimatedByMouse.Add (_selected);
            }
        }

        private bool IsAdjacentToSelectedElement ()
        {
            return _selected.X == this.X && Math.Abs (_selected.Y - this.Y) == 1 ||
            _selected.Y == this.Y && Math.Abs (_selected.X - this.X) == 1;
        }

        public static void AnimationByMouseMovement (int x, int y)
        {
            int xx = x - Game1.INDENT;
            int yy = y - Game1.INDENT;
            if (xx >= Game1.GAMEFIELD || yy >= Game1.GAMEFIELD || xx < 0 || yy < 0) {
                ElementUnderMouse = null;
            } else {
                try {
                    ElementUnderMouse = GameProcess.ListElements [xx / Game1.WIDTH] [yy / Game1.WIDTH];
                } catch (NullReferenceException) {
                    return;
                }
                if (!AnimatedByMouse.Contains (ElementUnderMouse))
                    AnimatedByMouse.Add (ElementUnderMouse);
            }
        }

        public static void CountMouseAnimation ()
        {
            List <Element> _ = new  List <Element> (AnimatedByMouse);
            foreach (var e in _) {
                e.UnderMouseAnimation ();
            } 
            
        }

        public void UnderMouseAnimation ()
        {
            if ((this == _selected || this.IsBlocked) && Rate <= 1.5)
                Rate += 0.07f;
            if (ElementUnderMouse == this && Rate <= 1.15)
                Rate += 0.03f;
            else if (!this.IsBlocked && ElementUnderMouse != this && Rate > 1)
                Rate -= 0.01f;
            else if (Rate < 1) {
                Rate += 0.06f;
            }
            if (Math.Abs (Rate - 0.99f) < 0.01) {
                Rate = 1;
                AnimatedByMouse.Remove (this);
            }
        }

        /// <summary>
        /// Determines what the action we should choose after this click in case if we already have selected element
        /// </summary>
        /// <returns><c>-1</c>, Should drop the selection, <c>0</c> Try to swap, <c>1</c> Select another one</returns>
        /// <param name="e">The clicked element </param>
        public int WhatForClick (Element e)
        {
            if (_selected.X == e.X) {
                if (Math.Abs (_selected.Y - e.Y) == 0)
                    return -1;
                if (Math.Abs (_selected.Y - e.Y) == 1)
                    return 0;
            }

            if (_selected.Y == e.Y) {
                if (Math.Abs (_selected.X - e.X) == 0)
                    return -1;
                if (Math.Abs (_selected.X - e.X) == 1)
                    return 0;
            }
            return 1;
        }
    }
}