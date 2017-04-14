﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LED.Strip.Adressable
{
    public class LEDsData
    {
        /// <summary>
        /// The brightness of the leds in the strip.
        /// </summary>
        private byte _brightness;

        /// <summary>
        /// The rgb color of the leds in the led strip.
        /// </summary>
        private byte[] _rgb;

        /// <summary>
        /// The array of paces in the led strip.
        /// Each place is '0' for turning off or '1' for turning on.
        /// </summary>
        private byte[] _turnOnPlaces;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="brightness">The brightness for all leds in the led strip.</param>
        /// <param name="red">The red color paramatere for the leds in the leds strip.</param>
        /// <param name="green">The green color paramatere for the leds in the leds strip.</param>
        /// <param name="blue">The blue color paramatere for the leds in the leds strip.</param>
        /// <param name="turnedOnPlaces">all the places in the led strip with their value for turning on/off.</param>
        public LEDsData(byte brightness , byte red , byte green ,byte blue , byte [] turnedOnPlaces)
        {
            _rgb = new byte[3];
            _brightness = brightness;
            _rgb[0] = red;
            _rgb[1] = green;
            _rgb[2] = blue;
        }

        /// <summary>
        /// Set or Get the array of paces in the led strip.
        /// Each place is '0' for turning off or '1' for turning on.
        /// </summary>
        public byte[] TurnedOnPlaces { get { return _turnOnPlaces; } set { _turnOnPlaces = value; } }

        /// <summary>
        /// Set or Get the red color paramatere for the leds in the leds strip.
        /// </summary>
        public byte Red { get { return _rgb[0]; } set { _rgb[0] = value; } }

        /// <summary>
        /// Set or Get the green color paramatere for the leds in the leds strip.
        /// </summary>
        public byte Green { get { return _rgb[1]; } set { _rgb[1] = value; } }

        /// <summary>
        /// Set or Get the blue color paramatere for the leds in the leds strip.
        /// </summary>
        public byte Blue { get { return _rgb[2]; } set { _rgb[2] = value; } }
    }
}
