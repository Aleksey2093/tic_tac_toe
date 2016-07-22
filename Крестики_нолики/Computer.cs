using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Крестики_нолики
{
    class Computer
    {
        /*0 1 2
         *3 4 5
         *6 7 8
         */
        public int hod_comp_main(int[] mass, int vrah)
        {
            //массив отвечает за поле. 3 - пустая ячейка, 1 - стоит крестик, 0 - стоит нолик
            if (mass.AsParallel().All(x => x == 3))
                return 4;
            int me = (vrah == 1) ? 0 : 1;
            int[][] matrix = getMatrix(mass);
            int res = provPobeda((int[][])matrix.Clone(),me,vrah);
            if (res != -1)
                return res;
            matrix = getMatrix(mass);
            res = provVrag((int[][])matrix.Clone(), me,vrah);
            if (res != -1)
                return res;
            matrix = getMatrix(mass);
            res = randromLogikHod((int[][])matrix.Clone(),(int[])mass.Clone(),me,vrah);
            return res;
        }

        /// <summary>
        /// Проверка победы по всем признакам
        /// </summary>
        /// <param name="matrix">Поле</param>
        /// <param name="me">Я</param>
        /// <param name="vrah">Соперник</param>
        /// <returns></returns>
        private int provPobeda(int[][] matrix, int me, int vrah)
        {
            int res = provPobedaCub((int[][])matrix.Clone(), me, vrah);
            return (res != -1) ? res : provPobedaDiagolnal(matrix, me, vrah);
        }

        /// <summary>
        /// Проверка победы по диагонали
        /// </summary>
        /// <param name="matrix">Поле</param>
        /// <param name="me">Я</param>
        /// <param name="vrah">Соперник</param>
        /// <returns></returns>
        private int provPobedaDiagolnal(int[][] matrix, int me, int vrah)
        {
            int res = 0;
            for (int i = 0; i < 3; i++)
            {
                if (matrix[i][i] == vrah) { res = 0; break; }
                else if (matrix[i][i] == me) { res += 1; }
                else if (matrix[i][i] != 3) { return (i * 3 + i) * (-1); }
            }
            if (res == 2)
            {
                if (matrix[0][0] == 3) return 0;
                if (matrix[1][1] == 3) return 4;
                if (matrix[2][2] == 3) return 8;
            }
            res = 0;
            for (int i=0;i<3;i++)
            {
                int j = 2 - i;
                if (matrix[i][j] == vrah) { res = 0; break; }
                else if (matrix[i][j] == me) { res += 1; }
                else if (matrix[i][j] != 3) { return (i * 3 + j) * (-1); }
            }
            if (res == 2)
            {
                //0 1 2
                //3 4 5
                //6 7 8
                if (matrix[0][2] == 3) return 2;
                if (matrix[1][1] == 3) return 4;
                if (matrix[2][0] == 3) return 6;
            }
            return -1;
        }

        enum Cell { X = 1, O = 0, NULL = 3 };

        /// <summary>
        /// Рандомный логический ход
        /// </summary>
        /// <param name="matrix">Матрица поля</param>
        /// <param name="mass">Массив поля</param>
        /// <param name="me">Я</param>
        /// <param name="vrah">Соперник</param>
        /// <returns></returns>
        private int randromLogikHod(int[][] matrix, int[] mass, int me, int vrah)
        {
            //0 1 2
            //3 4 5
            //6 7 8
            if (mass[4] == 3)
                return 4;
            if (mass[0] == 3)
                return 0;
            if (mass[2] == 3)
                return 2;
            if (mass[6] == 3)
                return 6;
            if (mass[8] == 3)
                return 8;
            Random rand;
            try { int chisel = (DateTime.Now - new DateTime(2016, 7, 22)).Seconds; rand = new Random(chisel); }
            catch (Exception) { rand = new Random(DateTime.Now.Millisecond); }
            DateTime start = DateTime.Now;
            while(true)
            {
                int i = rand.Next(0, 9);
                if (mass[i] == 3)
                    return i;
                if ((DateTime.Now - start).Seconds >= 60)
                    break;
            }
            for (int i=0;i<9;i++)
            {
                if (mass[i] == 3)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Проверка на победу по линиям
        /// </summary>
        /// <param name="pole">Игровое поле</param>
        /// <param name="me">Я</param>
        /// <param name="vrah">Соперник</param>
        /// <returns></returns>
        private int provPobedaCub(int[][] pole, int me, int vrah)
        {
            bool start = true;
            start1:
            if (me == 1)
            {
                for (int i = 0; i < 3; i++)
                {
                    int kol = 0;
                    for (int j = 0; j < 3; j++)
                    {
                        switch (pole[i][j])
                        {
                            case 0:
                                goto ret1;
                            case 1:
                                kol++;
                                break;
                            case 3:
                                break;
                            default:
                                return ((i * 3 + j) * (-1));
                        }
                    }
                    if (kol == 2)
                    {
                        for (int j = 0; j < 3; j++)
                           if (pole[i][j] == 3)
                                return i * 3 + j;
                    }
                    ret1:;
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    int kol = 0;
                    for (int j = 0; j < 3; j++)
                    {
                        switch (pole[i][j])
                        {
                            case 1:
                                goto ret1;
                            case 0:
                                kol++;
                                break;
                            case 3:
                                break;
                            default:
                                return ((i * 3 + j) * (-1));
                        }
                    }
                    if (kol == 2)
                    {
                        for (int j = 0; j < 3; j++)
                            if (pole[i][j] == 3)
                                return i * 3 + j;
                    }
                    ret1:;
                }
            }
            if (start == false)
                return -1;
            start = false;
            pole = matrixRight(pole);
            goto start1;
        }

        /// <summary>
        /// Проверка на проигрыш
        /// </summary>
        /// <param name="pole">Игровое поле</param>
        /// <param name="me">Я</param>
        /// <param name="vrah">Враг</param>
        /// <returns></returns>
        private int provVrag(int[][] pole, int me, int vrah)
        {
            int res = provVragCub((int[][])pole.Clone(), me, vrah);
            return (res != -1) ? res : provVragDiagol((int[][])pole.Clone(), me, vrah); 
        }

        private int provVragCub(int[][] pole, int me, int vrah)
        {
            for (int i=0;i<3;i++)
            {
                int kol = 0;
            }
        }

        private int provVragDiagol(int[][] pole, int me, int vrah)
        {
            int res = 0;

            for (int i = 0; i < 3; i++)
                if (pole[i][i] == vrah)
                    res += 1;
                else goto ret1;

            if (res == 2)
                for (int i=0;i<3;i++) if (pole[i][i] == 3) return (i * 3 + i);

            ret1:

            for (int i = 0; i < 3; i++)
                if (pole[i][2-i] == vrah)
                    res += 1;
                else return -1;

            if (res == 2)
                for (int i = 0; i < 3; i++) if (pole[i][2-i] == 3) return (i * 3 + (2-i));
            System.Windows.Forms.MessageBox.Show("Test");
            return -1;
        }

        /// <summary>
        /// Повернутая направо матрица
        /// </summary>
        /// <param name="original">Исходная матрица</param>
        /// <returns></returns>
        private int[][] matrixRight(int[][] original)
        {
            int[][] res = new int[3][] { new int[3], new int[3], new int[3] };
            Parallel.Invoke(() =>
            {
                res[0][0] = original[2][0];
                res[0][1] = original[1][0];
                res[0][2] = original[0][0];
            }, () =>
            {
                res[1][0] = original[2][1];
                res[1][1] = original[1][1];
                res[1][2] = original[0][1];
            }, () =>
            {
                res[2][0] = original[2][2];
                res[2][1] = original[1][2];
                res[2][2] = original[0][2];
            });
            return res;
        }
        /*0 1 2
         * 3 4 5
         * 6 7 8
         * 
         * 6 3 0
         * 7 4 1
         * 8 5 2
         */

        /// <summary>
        /// Получить матрицу 3х3 из массива х9
        /// </summary>
        /// <param name="original">Массив</param>
        /// <returns></returns>
        private int[][] getMatrix(int[] original)
        {
            int[][] res = new int[3][] { new int[3], new int[3], new int[3] };
            Parallel.For(0, 3, (i, state) =>
            {
                for (int j = 0; j < 3; j++)
                    res[i][j] = original[i * 3 + j];
            });
            return res;
        }
    }
}
