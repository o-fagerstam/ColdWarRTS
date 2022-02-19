﻿using System;
namespace Math {
    public class MersenneTwister {
// Class MersenneTwister generates random numbers
// from a uniform distribution using the Mersenne
// Twister algorithm.
// Taken from https://www.codeproject.com/Articles/164087/Random-Number-Generation
// Code Project Open License (CPOL)
        private const int N = 624;
        private const int M = 397;
        private const uint MATRIX_A = 0x9908b0dfU;
        private const uint UPPER_MASK = 0x80000000U;
        private const uint LOWER_MASK = 0x7fffffffU;
        private const int MAX_RAND_INT = 0x7fffffff;
        private uint[] _mag01 = {0x0U, MATRIX_A};
        private uint[] _mt = new uint[N];
        private int _mti = N+1;
        public MersenneTwister()
        { init_genrand( (uint)DateTime.Now.Millisecond); }
        public MersenneTwister( int seed )
        {
            init_genrand( (uint)seed );
        }

        public MersenneTwister( int[] init )
        {
            uint[] initArray = new uint[init.Length];
            for ( int i = 0; i < init.Length; ++i )
                initArray[i] = (uint)init[i];
            init_by_array( initArray, (uint)initArray.Length);
        }
        public static int MaxRandomInt
        { get { return 0x7fffffff; } }
        public int Next()
        { return genrand_int31(); }
        public int Next( int maxValue )
        { return Next( 0, maxValue ); }
        public int Next( int minValue, int maxValue )
        {
            if ( minValue > maxValue )
            {
                int tmp = maxValue;
                maxValue = minValue;
                minValue = tmp;
            }
            return (int)(System.Math.Floor((maxValue-minValue+1)*genrand_real1()+
                                           minValue));
        }
        public float NextFloat()
        { return (float) genrand_real2(); }
        public float NextFloat( bool includeOne )
        {
            if ( includeOne )
            {
                return (float) genrand_real1();
            }
            return (float) genrand_real2();
        }
        public float NextFloatPositive()
        { return (float) genrand_real3(); }
        public double NextDouble()
        { return genrand_real2(); }
        public double NextDouble( bool includeOne )
        {
            if ( includeOne )
            {
                return genrand_real1();
            }
            return genrand_real2();
        }

        public double NextDoublePositive()
        { return genrand_real3(); }
        public double Next53BitRes()
        { return genrand_res53(); }
        public void Initialize()
        { init_genrand((uint)DateTime.Now.Millisecond); }
        public void Initialize( int seed )
        { init_genrand( (uint)seed ); }
        public void Initialize( int[] init )
        {
            uint[] initArray = new uint[init.Length];
            for ( int i = 0; i < init.Length; ++i )
                initArray[i] = (uint)init[i];
            init_by_array( initArray, (uint)initArray.Length );
        }
        private void init_genrand( uint s)
        {
            _mt[0]= s & 0xffffffffU;
            for (_mti=1; _mti<N; _mti++)
            {
                _mt[_mti] = (uint)(1812433253U*(_mt[_mti-1]^(_mt[_mti-1]>>30))+_mti);
                _mt[_mti] &= 0xffffffffU;
            }
        }
        private void init_by_array(uint[] initKey, uint keyLength)
        {
            int i, j, k;
            init_genrand(19650218U);
            i=1; j=0;
            k = (int)(N>keyLength ? N : keyLength);
            for (; k>0; k--)
            {
                _mt[i] = (uint)((uint)(_mt[i]^((_mt[i-1]^(_mt[i-1]>>30))*1664525U))+initKey[j]+j);
                _mt[i] &= 0xffffffffU;
                i++; j++;
                if (i>=N) { _mt[0] = _mt[N-1]; i=1; }
                if (j>=keyLength) j=0;
            }
            for (k=N-1; k>0; k--)
            {
                _mt[i] = (uint)((uint)(_mt[i] ^ ((_mt[i-1] ^ (_mt[i-1] >> 30)) *
                                               1566083941U))- i);
                _mt[i] &= 0xffffffffU;
                i++;
                if (i>=N) { _mt[0] = _mt[N-1]; i=1; }
            }
            _mt[0] = 0x80000000U;
        }

        uint genrand_int32()
        {
            uint y;
            if (_mti >= N)
            {
                int kk;
                if (_mti == N+1)
                    init_genrand(5489U);
                for (kk=0;kk<N-M;kk++)
                {
                    y = (_mt[kk]&UPPER_MASK)|(_mt[kk+1]&LOWER_MASK);
                    _mt[kk] = _mt[kk+M] ^ (y >> 1) ^ _mag01[y & 0x1U];
                }
                for (;kk<N-1;kk++)
                {
                    y = (_mt[kk]&UPPER_MASK)|(_mt[kk+1]&LOWER_MASK);
                    _mt[kk] = _mt[kk+(M-N)] ^ (y >> 1) ^ _mag01[y & 0x1U];
                }
                y = (_mt[N-1]&UPPER_MASK)|(_mt[0]&LOWER_MASK);
                _mt[N-1] = _mt[M-1] ^ (y >> 1) ^ _mag01[y & 0x1U];
                _mti = 0;
            }
            y = _mt[_mti++];
            y ^= (y >> 11);
            y ^= (y << 7) & 0x9d2c5680U;
            y ^= (y << 15) & 0xefc60000U;
            y ^= (y >> 18);
            return y;
        }
        private int genrand_int31()
        { return (int)(genrand_int32()>>1); }
        double genrand_real1()
        { return genrand_int32()*(1.0/4294967295.0); }
        double genrand_real2()
        { return genrand_int32()*(1.0/4294967296.0); }
        double genrand_real3()
        {
            return (((double)genrand_int32())+0.5)*(1.0/4294967296.0);}
        double genrand_res53()
        {
            uint a=genrand_int32()>>5, b=genrand_int32()>>6;
            return(a*67108864.0+b)*(1.0/9007199254740992.0);
        }
    }
    // public class Program {
    //     static void Main()
    //     {
    //         MersenneTwister randGen = new MersenneTwister();
    //         Console.WriteLine( "100 uniform random integers in [0,{0}]:",
    //             MersenneTwister.MaxRandomInt);
    //         int i;
    //
    //         for (i = 0; i < 100; ++i)
    //         {
    //             Console.Write("{0} ",randGen.Next());
    //             if ( i%5 == 4 ) Console.WriteLine("");
    //         }
    //         Console.WriteLine("100 uniform random doubles in [0,1]:");
    //         for ( i = 0; i < 100; ++i )
    //         {
    //             Console.Write("{0} ",randGen.NextDouble().ToString("F8"));
    //             if ( i%5 == 4 ) Console.WriteLine("");
    //         }
    //         Console.WriteLine("Press ENTER to quit");
    //         Console.ReadLine();
    //     }
    // }
}