using System;
using System.Text;


namespace Process
{


    public static class dbConsultation
    {

        public static string getAvailableThreshold( )
        {
            lock (typeof( Process.dbConsultation) )
            {
                Entity.Entities.Prime_sequence primes = new Entity.Entities.Prime_sequence();
                primes.GetActualOrdinal();// the last
                Int64 availableOrdinal = primes.getCurrentOrdinal();
                Int64 availablePrime = primes.getCurrentPrime();
                //
                string message =
                    "\r\n prime = " + Process.Consultation.fillWithDots(availablePrime.ToString())
                    + "  its ordinal in the Primes sequence is= " + Process.Consultation.fillWithDots(availableOrdinal.ToString());
                //
                return message;
            }// end critical section.
        }//




        /// <summary>
        /// read at specified ordinal.
        /// </summary>
        public static string readAtSpecifiedOrdinal( Int64 specifiedOrdinal)
        {
            Entity.Entities.Prime_sequence boundaries = new Entity.Entities.Prime_sequence();
            boundaries.GetActualOrdinal();// the last
            Int64 availableOrdinal = boundaries.getCurrentOrdinal();
            Int64 availablePrime = boundaries.getCurrentPrime();
            //
            Entity.Entities.Prime_sequence prime_seq = new Entity.Entities.Prime_sequence( specifiedOrdinal);
            string message;
            //
            if (
                0L < specifiedOrdinal
                && availableOrdinal >= specifiedOrdinal
                )
            {
                prime_seq.dbSeekingEngine( specifiedOrdinal);
                message =
                    "\r\n prime = " + Process.Consultation.fillWithDots( prime_seq.getCurrentPrime().ToString())
                    + "  its ordinal in the Primes sequence is= " + Process.Consultation.fillWithDots(prime_seq.getCurrentOrdinal().ToString());
            }
            else
            {
                message = "\r\n Wrong input. Natural number required( i.e. >=1).";
            }
            // ready
            return message;
        }//


        public static string readInOrdinalRange( Int64 min, Int64 max)
        {
            Entity.Entities.Prime_sequence primes = new Entity.Entities.Prime_sequence( min, max);
            Int64 availableOrdinal = primes.GetActualOrdinal();
            string message = "";
            if (
                (0L < min)
                && min <= max
                && max <= availableOrdinal
              )
            {
                Entity.Entities.Prime_sequence.SelectedCouple[] retrievedSequence =
                    primes.LoadRange();
                //
                for (int c = 0; c < retrievedSequence.Length; c++)
                {
                    message +=
                        "\r\n prime = " + Process.Consultation.fillWithDots(retrievedSequence[c].Prime.ToString())
                        + "  its ordinal in the Primes sequence is= " + Process.Consultation.fillWithDots(retrievedSequence[c].atOrdinal.ToString());
                }
            }
            else
            {
                message = "\r\n Wrong input. Natural number required( i.e. >=1), min<=max.";
            }
            //
            return message;
        }//


    }//


}
