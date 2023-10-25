namespace AutomatedCar.SystemComponents
{
    using AutomatedCar.Models;
    using Avalonia.Media;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class Sensor
    {
        public double viewAngle { get; set; }

        public double viewDistance { get; set; }

        public List<WorldObject> currentObjectinView { get; set; }

        // kell az összehasonlításhoz:
        public List<WorldObject> previousObjectinView { get; set; }

        public List<RelevantObject> previousRelevant { get; set; }

        public PolylineGeometry sensorTriangle { get; set; }

        public Sensor(double ViewAngle, double ViewDistance)
        {
            viewAngle = ViewAngle;
            viewDistance = ViewDistance;
        }
        public void SensorTriangleUpdate()
        {
            //frissíteni kell a previousObjectinView listát
        }

        public List<RelevantObject> RelevantObjects()
        {
            // struct lista amit majd vissza adunk
            List<RelevantObject> ret = new List<RelevantObject>();
            // most vizsgálandó WO-k listája
            List<WorldObject> currentlyRelevant = new List<WorldObject>();

            // kiszedjük a most vizsgálandó listába azokat a WO-kat, amik előbb is és most is látótérben vannak, azaz potenciálisan relevánsak lehetnek
            foreach (WorldObject rev in this.currentObjectinView)
            {
                if (previousObjectinView.Contains(rev))
                {
                    currentlyRelevant.Add(rev);
                }
            }

            // végig megyünk a korábban releváns(!) elemek listáján, mindegyikre megnézzük, hogy most is releváns lehet e
            foreach (RelevantObject rev2 in previousRelevant)
            {
                foreach (WorldObject WO in currentlyRelevant)
                {
                    // ha most is releváns és közelített az autóhoz, akkor módosítjuk a távolságát, hogy az aktuális értéket mutassa és berakjuk a vissza adandók listájába
                    if (rev2.Object.Equals(WO) && rev2.ObjectDistance > this.examDistance(WO))
                    {
                        rev2.modifyDistance(this.examDistance(WO)); // (itt azért így kell, mert foreach elemet nem enged módosítani)
                        ret.Add(rev2);
                    }
                    // ha most nem releváns, mert nincs a látótérben, vagy mert nem közelített az autóhoz, akkor eltávolítjuk a korábban relevánsok listájából
                    else
                    {
                        currentlyRelevant.Remove(WO); // (ez csak optimalizáció miatt van itt, hogy akkor ha már egy object-ről tudjuk,
                                                      // hogy nem kell már vizsgálni, a belső foreach ne fusson rajta végig mégegyszer)
                        this.previousRelevant.Remove(rev2);
                    }
                }
            }

            return ret;
        }

        public double examDistance(WorldObject WO)
        {
            // itt vlaszeg van valamilyen szebb megoldás
            foreach (RelevantObject rev in this.previousRelevant)
            {
                if (rev.Object.Equals(WO))
                {
                    return rev.ObjectDistance;
                }
            }

            return 999; // akkorát adunk visza, hogy mindenképp nagyobb legyen
        }


    }
}
