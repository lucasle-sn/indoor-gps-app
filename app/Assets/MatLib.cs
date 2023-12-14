/*
 * The University of Melbourne
 * School of Engineering
 * MCEN90032 Sensor Systems
 * Author: Quang Trung Le (987445)
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MatLib : MonoBehaviour
{
    private const double r2d = (180 / Math.PI);

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public static double[,] Zeros(int size)
    {
        double[,] result = new double[size, size];

        for (int i = 0; i < size; i++)
            result[i, i] = 0;

        return result;
    }

    public static double[,] Eyes(int size)
    {
        double[,] result = new double[size, size];

        for (int i = 0; i < size; i++)
            result[i, i] = 1;

        return result;
    }

    public static double[,] MultiplyMatrix(double[,] mat1, double[,] mat2)
    {
        int rowMat1 = mat1.GetLength(0);
        int colMat1 = mat1.GetLength(1);

        int rowMat2 = mat2.GetLength(0);
        int colMat2 = mat2.GetLength(1);

        double[,] result = new double[rowMat1, colMat2];

        if (colMat1 == rowMat2)
        {
            for (int i = 0; i < rowMat1; i++)
            {
                for (int j = 0; j < colMat2; j++)
                {
                    double resultij = 0;
                    for (int k = 0; k < colMat1; k++)
                    {
                        resultij = resultij + mat1[i, k] * mat2[k, j];
                    }
                    result[i, j] = resultij;
                }
            }
        }

        return result;
    }

    public static double[,] GetSkewSymMatrix(double[,] sw)
    {
        double wx = sw[1, 0];
        double wy = sw[2, 0];
        double wz = sw[3, 0];

        double[,] Ohm = { { 0, -wx, -wy, -wz }, { wx, 0, wz, -wy }, { wy, -wz, 0, wx }, { wz, wy, -wx, 0 } };

        return Ohm;
    }

    public static double[,] MultiplyMatrix(double scalar, double[,] mat)
    {
        int row = mat.GetLength(0);
        int col = mat.GetLength(1);

        double[,] result = new double[row, col];

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
                result[i, j] = mat[i, j] * scalar;
        }

        return result;

    }

    public static double[,] AddMatrix(double[,] mat1, double[,] mat2, int sign)
    {
        int rowMat1 = mat1.GetLength(0);
        int colMat1 = mat1.GetLength(1);

        int rowMat2 = mat2.GetLength(0);
        int colMat2 = mat2.GetLength(1);

        double[,] result = new double[rowMat1, colMat1];

        if (rowMat1 == rowMat2 && colMat1 == colMat2)
        {
            for (int i = 0; i < rowMat1; i++)
            {
                for (int j = 0; j < colMat1; j++)
                {
                    result[i, j] = mat1[i, j] + mat2[i, j] * sign;
                }
            }
        }

        return result;
    }

    public static double[,] Transpose(double[,] C)
    {
        int row = C.GetLength(0);
        int col = C.GetLength(1);

        double[,] result = new double[col, row];

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                result[i, j] = C[j, i];
            }
        }

        return result;
    }

    public static double[,] Cross(double[,] vec1, double[,] vec2)
    {
        double[,] result = new double[3, 1];

        int rowVec1 = vec1.GetLength(0);
        int colVec1 = vec1.GetLength(1);

        int rowVec2 = vec2.GetLength(0);
        int colVec2 = vec2.GetLength(1);

        if (colVec1 == 1 && colVec2 == 1 && rowVec1 == 3 && rowVec2 == 3)
        {
            result[0, 0] = vec1[1, 0] * vec2[2, 0] - vec1[2, 0] * vec2[1, 0];
            result[1, 0] = vec1[2, 0] * vec2[0, 0] - vec1[0, 0] * vec2[2, 0];
            result[2, 0] = vec1[0, 0] * vec2[1, 0] - vec1[1, 0] * vec2[0, 0];
        }

        return result;
    }

    public static double[,] Normalize(double[,] vec)
    {
        double[,] result = new double[3, 1];

        int row = vec.GetLength(0);
        int col = vec.GetLength(1);

        if (row == 3 && col == 1)
        {
            double mag = Math.Sqrt(vec[0, 0] * vec[0, 0] + vec[1, 0] * vec[1, 0] + vec[2, 0] * vec[2, 0]);
            result[0, 0] = vec[0, 0] / mag;
            result[1, 0] = vec[1, 0] / mag;
            result[2, 0] = vec[2, 0] / mag;
        }

        return result;
    }

    public static double[,] GetDCMQuaternion(double[,] q)
    {
        double[,] C = Eyes(3);

        int row = q.GetLength(0);
        int col = q.GetLength(1);

        if (row == 4 && col == 1)
        {
            double q0 = q[0, 0];
            double q1 = q[1, 0];
            double q2 = q[2, 0];
            double q3 = q[3, 0];

            C[0, 0] = q0 * q0 + q1 * q1 - q2 * q2 - q3 * q3;
            C[0, 1] = 2 * (q1 * q2 + q0 * q3);
            C[0, 2] = 2 * (q1 * q3 - q0 * q2);
            C[1, 0] = 2 * (q1 * q2 - q0 * q3);
            C[1, 1] = q0 * q0 - q1 * q1 + q2 * q2 - q3 * q3;
            C[1, 2] = 2 * (q2 * q3 + q0 * q1);
            C[2, 0] = 2 * (q1 * q3 + q0 * q2);
            C[2, 1] = 2 * (q2 * q3 - q0 * q1);
            C[2, 2] = q0 * q0 - q1 * q1 - q2 * q2 + q3 * q3;
        }

        return C;
    }

    public static double[,] GetDCMEuler(double theta, double phi, double psi)
    {
        theta = theta / r2d;
        phi = phi / r2d;
        psi = psi / r2d;

        double stheta = Math.Sin(theta);
        double sphi = Math.Sin(phi);
        double spsi = Math.Sin(psi);
        double ctheta = Math.Cos(theta);
        double cphi = Math.Cos(phi);
        double cpsi = Math.Cos(psi);

        double[,] C = new double[3, 3];
        C[0, 0] = cphi * cpsi - spsi * stheta * sphi;
        C[0, 1] = cphi * spsi + cpsi * stheta * sphi;
        C[0, 2] = -ctheta * sphi;
        C[1, 0] = -spsi * ctheta;
        C[1, 1] = ctheta * cpsi;
        C[1, 2] = stheta;
        C[2, 0] = sphi * cpsi + spsi * stheta * cphi;
        C[2, 1] = sphi * spsi - cpsi * stheta * cphi;
        C[2, 2] = ctheta * cphi;


        //double[,] C32 = { { Math.Cos(phi), 0, -Math.Sin(phi) }, { 0, 1, 0 }, { Math.Sin(phi), 0, Math.Cos(phi) } };
        //double[,] C21 = { { 1, 0, 0 }, { 0, Math.Cos(theta), Math.Sin(theta) }, { 0, -Math.Sin(theta), Math.Cos(theta) } };
        //double[,] C10 = { { Math.Cos(psi), Math.Sin(psi), 0 }, { -Math.Sin(psi), Math.Cos(psi), 0 }, { 0, 0, 1 } };

        //double[,] C = MultiplyMatrix(MultiplyMatrix(C32, C21), C10);

        return C;
    }

    public static double[] GetEulerAngle(double[,] q)
    {
        int row = q.GetLength(0);
        int col = q.GetLength(1);

        double[] result = new double[3];

        if (row == 4 && col == 1)
        {
            double q0 = q[0, 0];
            double q1 = q[1, 0];
            double q2 = q[2, 0];
            double q3 = q[3, 0];

            double theta = Math.Asin(2 * (q2 * q3 + q0 * q1));
            double phi = Math.Atan2(-2 * (q1 * q3 - q0 * q2), (q0 * q0 - q1 * q1 - q2 * q2 + q3 * q3));
            double psi = Math.Atan2(-2 * (q1 * q2 - q0 * q3), (q0 * q0 - q1 * q1 + q2 * q2 - q3 * q3));

            result[0] = Math.Round(R2D(theta), 2);
            result[1] = Math.Round(R2D(phi), 2);
            result[2] = Math.Round(R2D(psi), 2);
        }


        if (row == 3 && col == 3)
        {
            double theta = Math.Asin(q[1,2]);
            double phi = Math.Atan2(-q[0,2], q[2,2]);
            double psi = Math.Atan2(-q[1,0], q[1,1]);

            result[0] = Math.Round(R2D(theta), 2);
            result[1] = Math.Round(R2D(phi), 2);
            result[2] = Math.Round(R2D(psi), 2);
        }

        return result;
    }

    public static double[,] CrossQuaternion(double[,] vec1, double[,] vec2)
    {
        double[,] result = new double[4, 1];

        int rowVec1 = vec1.GetLength(0);
        int colVec1 = vec1.GetLength(1);

        int rowVec2 = vec2.GetLength(0);
        int colVec2 = vec2.GetLength(1);

        if (rowVec1 == 4 && rowVec2 == 4 && colVec1 == 1 && colVec2 == 1)
        {
            double q0 = vec1[0, 0];
            double q1 = vec1[1, 0];
            double q2 = vec1[2, 0];
            double q3 = vec1[3, 0];

            double[,] Ohm = new double[,] { { q0, -q1, -q2, -q3 }, { q1, q0, q3, -q2 }, { q2, -q3, q0, q1 }, { q3, q2, -q1, q0 } };
            result = MultiplyMatrix(Ohm, vec2);
        }

        return result;
    }

    public static double[,] GetUnitQuaternion(double[,] C)
    {
        double[,] result = new double[4, 1];

        int row = C.GetLength(0);
        int col = C.GetLength(1);

        double q0 = 0, q1 = 0, q2 = 0, q3 = 0;
        if (row==3 && col == 3)
        {
            q0 = Math.Sqrt(1 + C[0, 0] + C[1, 1] + C[2, 2])/2;
            if (q0 == 0)
            {
                q1 = Math.Sqrt(1 + C[0, 0] - C[1, 1] - C[2, 2])/2;
                q2 = (C[0, 1] + C[1, 0]) / (4 * q1);
                q3 = (C[2,0] + C[0,2]) / (4 * q1);
                q0 = (C[1, 2] - C[2, 1]) / (4 * q1);
            }
            else
            {
                q1 = (C[1, 2] - C[2, 1]) / (4 * q0);
                q2 = (C[2, 0] - C[0, 2]) / (4 * q0);
                q3 = (C[0, 1] - C[1, 0]) / (4 * q0);
            }
        }
        result[0, 0] = q0;
        result[1, 0] = q1;
        result[2, 0] = q2;
        result[3, 0] = q3;

        return result;
    }

    public static double R2D(double ang)
    {
        return (ang * r2d);
    }


    public static double[,] InvertMatrix44(double[,] mat)
    {
        double[,] result = new double[4, 4];

        int row = mat.GetLength(0);
        int col = mat.GetLength(1);

        if (row==4 && col == 4)
        {
            double[] m = new double[16];
            m[0] = mat[0, 0]; m[1] = mat[0, 1]; m[2] = mat[0, 2]; m[3] = mat[0, 3];
            m[4] = mat[1, 0]; m[5] = mat[1, 1]; m[6] = mat[1, 2]; m[7] = mat[1, 3];
            m[8] = mat[2, 0]; m[9] = mat[2, 1]; m[10] = mat[2, 2]; m[11] = mat[2, 3];
            m[12] = mat[3, 0]; m[13] = mat[3, 1]; m[14] = mat[3, 2]; m[15] = mat[3, 3];

            double[] inv = new double[16];

            inv[0] = m[5] * m[10] * m[15] - m[5] * m[11] * m[14] - m[9] * m[6] * m[15] + m[9] * m[7] * m[14] + m[13] * m[6] * m[11] - m[13] * m[7] * m[10];
            inv[4] = -m[4] * m[10] * m[15] + m[4] * m[11] * m[14] + m[8] * m[6] * m[15] - m[8] * m[7] * m[14] - m[12] * m[6] * m[11] + m[12] * m[7] * m[10];
            inv[8] = m[4] * m[9] * m[15] - m[4] * m[11] * m[13] - m[8] * m[5] * m[15] + m[8] * m[7] * m[13] + m[12] * m[5] * m[11] - m[12] * m[7] * m[9];
            inv[12] = -m[4] * m[9] * m[14] + m[4] * m[10] * m[13] + m[8] * m[5] * m[14] - m[8] * m[6] * m[13] - m[12] * m[5] * m[10] + m[12] * m[6] * m[9];
            inv[1] = -m[1] * m[10] * m[15] + m[1] * m[11] * m[14] + m[9] * m[2] * m[15] - m[9] * m[3] * m[14] - m[13] * m[2] * m[11] + m[13] * m[3] * m[10];
            inv[5] = m[0] * m[10] * m[15] - m[0] * m[11] * m[14] - m[8] * m[2] * m[15] + m[8] * m[3] * m[14] + m[12] * m[2] * m[11] - m[12] * m[3] * m[10];
            inv[9] = -m[0] * m[9] * m[15] + m[0] * m[11] * m[13] + m[8] * m[1] * m[15] - m[8] * m[3] * m[13] - m[12] * m[1] * m[11] + m[12] * m[3] * m[9];
            inv[13] = m[0] * m[9] * m[14] - m[0] * m[10] * m[13] - m[8] * m[1] * m[14] + m[8] * m[2] * m[13] + m[12] * m[1] * m[10] - m[12] * m[2] * m[9];
            inv[2] = m[1] * m[6] * m[15] - m[1] * m[7] * m[14] - m[5] * m[2] * m[15] + m[5] * m[3] * m[14] + m[13] * m[2] * m[7] - m[13] * m[3] * m[6];
            inv[6] = -m[0] * m[6] * m[15] + m[0] * m[7] * m[14] + m[4] * m[2] * m[15] - m[4] * m[3] * m[14] - m[12] * m[2] * m[7] + m[12] * m[3] * m[6];
            inv[10] = m[0] * m[5] * m[15] - m[0] * m[7] * m[13] - m[4] * m[1] * m[15] + m[4] * m[3] * m[13] + m[12] * m[1] * m[7] - m[12] * m[3] * m[5];
            inv[14] = -m[0] * m[5] * m[14] + m[0] * m[6] * m[13] + m[4] * m[1] * m[14] - m[4] * m[2] * m[13] - m[12] * m[1] * m[6] + m[12] * m[2] * m[5];
            inv[3] = -m[1] * m[6] * m[11] + m[1] * m[7] * m[10] + m[5] * m[2] * m[11] - m[5] * m[3] * m[10] - m[9] * m[2] * m[7] + m[9] * m[3] * m[6];
            inv[7] = m[0] * m[6] * m[11] - m[0] * m[7] * m[10] - m[4] * m[2] * m[11] + m[4] * m[3] * m[10] + m[8] * m[2] * m[7] - m[8] * m[3] * m[6];
            inv[11] = -m[0] * m[5] * m[11] + m[0] * m[7] * m[9] + m[4] * m[1] * m[11] - m[4] * m[3] * m[9] - m[8] * m[1] * m[7] + m[8] * m[3] * m[5];
            inv[15] = m[0] * m[5] * m[10] - m[0] * m[6] * m[9] - m[4] * m[1] * m[10] + m[4] * m[2] * m[9] + m[8] * m[1] * m[6] - m[8] * m[2] * m[5];

            double det = m[0] * inv[0] + m[1] * inv[4] + m[2] * inv[8] + m[3] * inv[12];

            if (det != 0)
            {
                det = 1 / det;

                double[] invOut = new double[16];
                for (int i = 0; i < 16; i++)
                    invOut[i] = inv[i] * det;

                result[0, 0] = invOut[0]; result[0, 1] = invOut[1]; result[0, 2] = invOut[2]; result[0, 3] = invOut[3];
                result[1, 0] = invOut[4]; result[1, 1] = invOut[5]; result[1, 2] = invOut[6]; result[1, 3] = invOut[7];
                result[2, 0] = invOut[8]; result[2, 1] = invOut[9]; result[2, 2] = invOut[10]; result[2, 3] = invOut[11];
                result[3, 0] = invOut[12]; result[3, 1] = invOut[13]; result[3, 2] = invOut[14]; result[3, 3] = invOut[15];
            }
        }

        return result;
    }



}
