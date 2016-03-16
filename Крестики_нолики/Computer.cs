using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Крестики_нолики
{
    class Computer
    {
        public int hod_comp_main(int[] mass, int vrah)
        {
            bool ye = true;
            for (int i = 0; i < 9; i++)
            { //если все поле пустое, то без раздумий ходим в центр
                if (mass[i] != 3) { ye = false; }
            }
            if (ye) { return 4; }//ход в центр
            else 
            {
            int res_center_null = -1, res_center_vrah = -1, res_center_mi = -1;
            if (mass[4] == 3) { res_center_null = center_null(mass, vrah); return res_center_null; }
            else if (mass[4] == vrah) { res_center_vrah = center_vrah(mass, vrah); return res_center_vrah; }
            else { res_center_mi = center_mi(mass, vrah); return res_center_mi; }
            }
            return -1; //ошибка
        }

        private int center_null(int[] mass, int vrah)
        {
            int c02 = 0, c06 = 0, c68 = 0, c28 = 0; //счетчики для противника
            int c02mi = 0, c06mi = 0, c68mi = 0, c28mi = 0; //счетчики для меня
            if (mass[0] == vrah) { c02++; c06++; } else if (mass[0] != 3) { c02mi++; c06mi++; }
            if (mass[1] == vrah) { c02++; } else if (mass[1] != 3) { c02mi++; }
            if (mass[2] == vrah) { c02++; c28++; } else if (mass[2] != 3) { c02mi++; c28mi++; }
            if (mass[3] == vrah) { c06++; } else if (mass[3] != 3) { c06mi++; }
            if (mass[5] == vrah) { c28++; } else if (mass[5] != 3) { c28mi++; }
            if (mass[6] == vrah) { c06++; c68++; } else if (mass[6] != 3) { c06mi++; c68mi++; }
            if (mass[7] == vrah) { c68++; } else if (mass[7] != 3) { c68mi++; }
            if (mass[8] == vrah) { c68++; c28++; } else if (mass[8] != 3) { c68mi++; c28mi++; }
            
            /*0 1 2
              3 4 5
              6 7 8*/

            if (c02 == 2 && c02mi == 0) //проверка критичных ситуаций
            {
                if (mass[1] == 3) return 1; //немедленно ходим туда
                if (mass[0] == 3) return 0;
                if (mass[2] == 3) return 2;
            }
            else if (c68 == 2 && c68mi == 0)
            {
                if (mass[6] == 3) return 6;
                if (mass[7] == 3) return 7;
                if (mass[8] == 3) return 8;
            }
            else if (c06 == 2 && c06mi == 0)
            {
                if (mass[0] == 3) return 0;
                if (mass[3] == 3) return 3;
                if (mass[6] == 3) return 6;
            }
            else if (c28 == 2 && c28mi == 0)
            {
                if (mass[2] == 3) return 0;
                if (mass[5] == 3) return 5;
                if (mass[8] == 3) return 8;
            }

            int uhol = 0;
            for (int i=0;i<9; i++)
            {
                if (mass[i] != 3) uhol++;
                if (uhol > 1) break;
            }
            if (uhol == 1)
            {
                if (mass[0] == vrah) return 8;
                if (mass[1] == vrah) return 7;
                if (mass[2] == vrah) return 6;
                if (mass[3] == vrah) return 5;
                if (mass[5] == vrah) return 3;
                if (mass[6] == vrah) return 2;
                if (mass[7] == vrah) return 1;
                if (mass[8] == vrah) return 0;
            }

            return 4; //если вокруг все удачно то идем в центр
        }
        private int center_vrah(int[] mass, int vrah)
        {
            bool rand = true;
            for (int i = 0; i < 9; i ++)
                if (i != 4 && mass[i] != 3) { rand = false; break; }
            if (rand) /* если все пусто то в угол */ return 0; /* левый верхний сойдет */

            int mi; if (vrah == 1) mi = 0; else mi = 1;
            /*if (mass[0] == mi && mass[8] == 3) { return 8; }
            if (mass[1] == mi && mass[7] == 3) { return 7; }
            if (mass[2] == mi && mass[6] == 3) { return 6; }
            if (mass[3] == mi && mass[5] == 3) { return 5; }
            if (mass[5] == mi && mass[3] == 3) { return 3; }
            if (mass[6] == mi && mass[2] == 3) { return 2; }
            if (mass[7] == mi && mass[1] == 3) { return 1; }
            if (mass[8] == mi && mass[0] == 3) { return 0; }*/

            int c02 = 0, c06 = 0, c68 = 0, c28 = 0; //счетчики для противника
            int c02mi = 0, c06mi = 0, c68mi = 0, c28mi = 0; //счетчики для меня
            if (mass[0] == vrah) { c02++; c06++; } else if (mass[0] != 3) { c02mi++; c06mi++; }
            if (mass[1] == vrah) { c02++; } else if (mass[1] != 3) { c02mi++; }
            if (mass[2] == vrah) { c02++; c28++; } else if (mass[2] != 3) { c02mi++; c28mi++; }
            if (mass[3] == vrah) { c06++; } else if (mass[3] != 3) { c06mi++; }
            if (mass[5] == vrah) { c28++; } else if (mass[5] != 3) { c28mi++; }
            if (mass[6] == vrah) { c06++; c68++; } else if (mass[6] != 3) { c06mi++; c68mi++; }
            if (mass[7] == vrah) { c68++; } else if (mass[7] != 3) { c68mi++; }
            if (mass[8] == vrah) { c68++; c28++; } else if (mass[8] != 3) { c68mi++; c28mi++; }
            if (c02 == 0 && c02mi == 2) //проверка победных ситуаций
            {
                if (mass[1] == 3) return 1; //немедленно ходим туда
                if (mass[0] == 3) return 0;
                if (mass[2] == 3) return 2;
            }
            if (c68 == 0 && c68mi == 2)
            {
                if (mass[6] == 3) return 6;
                if (mass[7] == 3) return 7;
                if (mass[8] == 3) return 8;
            }
            if (c06 == 0 && c06mi == 2)
            {
                if (mass[0] == 3) return 0;
                if (mass[3] == 3) return 3;
                if (mass[6] == 3) return 6;
            }
            if (c28 == 0 && c28mi == 2)
            {
                if (mass[2] == 3) return 0;
                if (mass[5] == 3) return 5;
                if (mass[8] == 3) return 8;
            }

            if (mass[0] == vrah && mass[8] == 3) { return 8; }
            if (mass[1] == vrah && mass[7] == 3) { return 7; }
            if (mass[2] == vrah && mass[6] == 3) { return 6; }
            if (mass[3] == vrah && mass[5] == 3) { return 5; }
            if (mass[5] == vrah && mass[3] == 3) { return 3; }
            if (mass[6] == vrah && mass[2] == 3) { return 2; }
            if (mass[7] == vrah && mass[1] == 3) { return 1; }
            if (mass[8] == vrah && mass[0] == 3) { return 0; }

            if (c02 == 2 && c02mi == 0) //проверка критичных ситуаций
            {
                if (mass[1] == 3) return 1; //немедленно ходим туда
                if (mass[0] == 3) return 0;
                if (mass[2] == 3) return 2;
            }
            if (c68 == 2 && c68mi == 0)
            {
                if (mass[6] == 3) return 6;
                if (mass[7] == 3) return 7;
                if (mass[8] == 3) return 8;
            }
            if (c06 == 2 && c06mi == 0)
            {
                if (mass[0] == 3) return 0;
                if (mass[3] == 3) return 3;
                if (mass[6] == 3) return 6;
            }
            if (c28 == 2 && c28mi == 0)
            {
                if (mass[2] == 3) return 0;
                if (mass[5] == 3) return 5;
                if (mass[8] == 3) return 8;
            }
            //если мы приперлись сюда и не сделали хода, то это подозрительно, поэтому ходим в один из углов
            for (int i = 0; i < 9; i++)
                if (mass[i] == 3) return i;

            return -1;
        }
        private int center_mi(int[] mass, int vrah)
        {
            bool rand = true;
            for (int i = 0; i < 9; i++)
                if (i != 4 && mass[i] != 3) { rand = false; break; }
            if (rand) { return 0; }
            int mi; if (vrah == 1) mi = 0; else mi = 1;
            if (mass[0] == mi && mass[8] == 3) { return 8; }
            if (mass[1] == mi && mass[7] == 3) { return 7; }
            if (mass[2] == mi && mass[6] == 3) { return 6; }
            if (mass[3] == mi && mass[5] == 3) { return 5; }
            if (mass[5] == mi && mass[3] == 3) { return 3; }
            if (mass[6] == mi && mass[2] == 3) { return 2; }
            if (mass[7] == mi && mass[1] == 3) { return 1; }
            if (mass[8] == mi && mass[0] == 3) { return 0; }

            /*if (mass[0] == vrah && mass[8] == 3) { return 8; }
            if (mass[1] == vrah && mass[7] == 3) { return 7; }
            if (mass[2] == vrah && mass[6] == 3) { return 6; }
            if (mass[3] == vrah && mass[5] == 3) { return 5; }
            if (mass[5] == vrah && mass[3] == 3) { return 3; }
            if (mass[6] == vrah && mass[2] == 3) { return 2; }
            if (mass[7] == vrah && mass[1] == 3) { return 1; }
            if (mass[8] == vrah && mass[0] == 3) { return 0; }*/

            int c02 = 0, c06 = 0, c68 = 0, c28 = 0; //счетчики для противника
            int c02mi = 0, c06mi = 0, c68mi = 0, c28mi = 0; //счетчики для меня
            if (mass[0] == vrah) { c02++; c06++; } else if (mass[0] != 3) { c02mi++; c06mi++; }
            if (mass[1] == vrah) { c02++; } else if (mass[1] != 3) { c02mi++; }
            if (mass[2] == vrah) { c02++; c28++; } else if (mass[2] != 3) { c02mi++; c28mi++; }
            if (mass[3] == vrah) { c06++; } else if (mass[3] != 3) { c06mi++; }
            if (mass[5] == vrah) { c28++; } else if (mass[5] != 3) { c28mi++; }
            if (mass[6] == vrah) { c06++; c68++; } else if (mass[6] != 3) { c06mi++; c68mi++; }
            if (mass[7] == vrah) { c68++; } else if (mass[7] != 3) { c68mi++; }
            if (mass[8] == vrah) { c68++; c28++; } else if (mass[8] != 3) { c68mi++; c28mi++; }
            if (c02 == 2 && c02mi == 0) //проверка критичных ситуаций
            {
                if (mass[1] == 3) return 1; //немедленно ходим туда
                if (mass[0] == 3) return 0;
                if (mass[2] == 3) return 2;
            }
            if (c68 == 2 && c68mi == 0)
            {
                if (mass[6] == 3) return 6;
                if (mass[7] == 3) return 7;
                if (mass[8] == 3) return 8;
            }
            if (c06 == 2 && c06mi == 0)
            {
                if (mass[0] == 3) return 0;
                if (mass[3] == 3) return 3;
                if (mass[6] == 3) return 6;
            }
            if (c28 == 2 && c28mi == 0)
            {
                if (mass[2] == 3) return 2;
                if (mass[5] == 3) return 5;
                if (mass[8] == 3) return 8;
            }

            for (int i = 0; i < 9; i++)
                if (mass[i] == 3) return i;

            return -1;
        }
        private int partical_hod(int[] mass, int vrah)
        {
            int mi;
            if (vrah == 1) mi = 0; else mi = 1;

            return -1;
        }
    }
}
