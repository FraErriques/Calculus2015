using System;
using System.Collections.Generic;
using System.Text;


namespace Process
{


    public static class Consultation
    {



        public static string fillWithDots(string input)
        {
            System.Text.StringBuilder sb = new StringBuilder(input.Length * 2);// some more space to fill with dots, each three.
            int acc = 0;
            for (int c = input.Length - 1; c >= 0; c--)
            {
                sb.Append(input[c]);
                ++acc;
                if (3 == acc)
                {
                    sb.Append('.');
                    acc = 0;// reset.
                }// else skip, since we are in a group.
            }
            // trim possible initial dot( eg. .999.888->999.888) and revert.
            System.Text.StringBuilder sa = new StringBuilder(input.Length * 2);// some more space to fill with dots, each three.
            for (int c = sb.Length - 1; c >= 0; c--)
            {
                if (sb.Length - 1 == c && '.' == sb[c])
                {
                    // nop(no_operation), i.e. just skip.
                }
                else
                {
                    sa.Append(sb[c]);
                }
            }// end revert
            //ready
            return sa.ToString();
        }//



        public static string writeCoupleOnBoard(
            PrimesFinder.Primes thePrimes)
        {
            //
            Int64 lastPrimeFound = thePrimes.getActualPrime();
            string str_lastPrimeFound = lastPrimeFound.ToString();
            string boardMessage = "\r\n prime = " + fillWithDots(str_lastPrimeFound);
            //
            string lastOrdinal = thePrimes.getActualOrdinal().ToString();
            boardMessage += "  its ordinal in the Primes sequence is= " + fillWithDots(lastOrdinal);
            // ready
            return boardMessage;
        }



        public static string getAvailableThreshold( )
        {
            PrimesFinder.Primes thePrimes = new PrimesFinder.Primes();
            string boardMessage;
            if (thePrimes.getCanOperateStatus())
            {
                boardMessage = writeCoupleOnBoard(thePrimes);
            }// else can do nothing
            else
            {
                boardMessage = "\r\n dump file busy: unable to read.";
            }
            thePrimes.Dispose();
            return boardMessage;
        }//



        /// <summary>
        /// read at specified ordinal.
        /// </summary>
        public static string readAtSpecifiedOrdinal(Int64 specifiedOrdinal)
        {
            PrimesFinder.Primes thePrimes = new PrimesFinder.Primes();
            string boardMessage;
            if (thePrimes.getCanOperateStatus())
            {
                if (
                    0L < specifiedOrdinal
                    && thePrimes.getActualOrdinal() >= specifiedOrdinal
                    )
                {
                    bool canSeek = thePrimes.SeekingEngine( specifiedOrdinal);
                    if (!canSeek) return "\r\n required ordinal is outside the actual sequence.";
                    boardMessage = writeCoupleOnBoard(thePrimes);
                }
                else
                {
                    boardMessage = "\r\n Wrong input. Natural number required( i.e. >=1).";
                }
            }// else can do nothing
            else
            {
                boardMessage = "\r\n dump file busy: unable to read.";
            }
            // anyway
            thePrimes.Dispose();
            // ready
            return boardMessage;
        }//


        public static string readInOrdinalRange( Int64 min, Int64 max)
        {
            PrimesFinder.Primes thePrimes = new PrimesFinder.Primes();
            string boardMessage = "";// will be appended.
            if (thePrimes.getCanOperateStatus())
            {
                if (
                    (0L < min)
                    && min <= max
                    && max <= thePrimes.getActualOrdinal()
                  )
                {
                    for (Int64 c = min; c <= max; c++)
                    {
                        bool canSeek = thePrimes.SeekingEngine(c);
                        if (!canSeek) return "\r\n required ordinal is outside the actual sequence.";
                        boardMessage += writeCoupleOnBoard( thePrimes);
                    }
                }
                else
                {
                    boardMessage = "\r\n Wrong input. Natural number required( i.e. >=1), min<=max.";
                }
            }// else can do nothing
            else
            {
                boardMessage = "\r\n dump file busy: unable to read.";
            }
            // anyway
            thePrimes.Dispose();
            // ready
            return boardMessage;
        }//


    }//


}
