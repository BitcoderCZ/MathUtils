using System;
using System.Collections.Generic;
using System.Text;

namespace MathUtils
{
    public struct Matrix4
    {
        private readonly double[] components;

        public double this[int index]
        {
            get => components[index];
            set => components[index] = value;
        }

        public Matrix4(double[] _components)
        {
            if (_components.Length != 16)
                throw new ArgumentException(nameof(_components));

            components = _components;
        }

        public static Matrix4 Rotation(Quaternion q)
        {
            double x2 = q.X + q.X, y2 = q.Y + q.Y, z2 = q.Z + q.Z;
            double xx = q.X * x2, xy = q.X * y2, xz = q.X * z2;
            double yy = q.Y * y2, yz = q.Y * z2, zz = q.Z * z2;
            double wx = q.W * x2, wy = q.W * y2, wz = q.W * z2;

            return new Matrix4(new double[16]
            {
                1f - (yy + zz),
                xy + wz,
                xz - wy,
                0f,

                xy - wz,
                1f - (xx + zz),
                yz + wx,
                0f,

                xz + wy,
                yz - wx,
                1f - (xx + yy),
                0f,

                0f,
                0f,
                0f,
                1f,
            });
        }
    }
}
