namespace MyEmmControl.Communication
{
    public partial class MyXbox
    {
        public class FilterKalman
        {
            private double A = 1;
            private double B = 0;
            private double H = 1;

            private double R;
            private double Q;

            private double cov = double.NaN;
            private double x = double.NaN;

            public FilterKalman(double R, double Q, double A, double B, double H)
            {
                this.R = R;  //过程噪声
                this.Q = Q;  //测量噪声

                this.A = A;  //状态转移矩阵
                this.B = B;  //控制矩阵  u为控制向量
                this.H = H;  //将估计范围与单位转化为与系统变量(或者说测量值)一致的范围与单位

                this.cov = double.NaN;
                this.x = double.NaN; // estimated signal without noise
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="R">过程噪声</param>
            /// <param name="Q">测量噪声</param>
            public FilterKalman(double R, double Q)
            {
                this.R = R;
                this.Q = Q;
            }

            public double filter(double measurement, double u)
            {
                if (double.IsNaN(this.x))
                {
                    this.x = (1 / this.H) * measurement;
                    this.cov = (1 / this.H) * this.Q * (1 / this.H);
                }
                else
                {
                    double predX = (this.A * this.x) + (this.B * u);
                    double predCov = ((this.A * this.cov) * this.A) + this.Q;

                    // Kalman gain
                    double K = predCov * this.H * (1 / ((this.H * predCov * this.H) + this.Q));

                    // Correction
                    this.x = predX + K * (measurement - (this.H * predX));
                    this.cov = predCov - (K * this.H * predCov);
                }
                return this.x;
            }

            public double filter(double measurement)
            {
                double u = 0;
                if (double.IsNaN(this.x))
                {
                    this.x = (1 / this.H) * measurement;
                    this.cov = (1 / this.H) * this.Q * (1 / this.H);
                }
                else
                {
                    double predX = (this.A * this.x) + (this.B * u);
                    double predCov = ((this.A * this.cov) * this.A) + this.R;

                    // Kalman gain
                    double K = predCov * this.H * (1 / ((this.H * predCov * this.H) + this.Q));

                    // Correction
                    this.x = predX + K * (measurement - (this.H * predX));
                    this.cov = predCov - (K * this.H * predCov);
                }
                return this.x;
            }

            public double lastMeasurement()
            {
                return this.x;
            }

            public void setMeasurementNoise(double noise)
            {
                this.Q = noise;
            }

            public void setProcessNoise(double noise)
            {
                this.R = noise;
            }
        }
    }
}
