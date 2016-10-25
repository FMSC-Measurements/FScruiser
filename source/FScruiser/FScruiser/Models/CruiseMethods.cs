﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    public static class CruiseMethods
    {
        public static readonly string H_PCT = "100";
        public static readonly string STR = "STR";
        public static readonly string S3P = "S3P";
        public static readonly string THREEP = "3P";
        public static readonly string FIX = "FIX";
        public static readonly string F3P = "F3P";
        public static readonly string FCM = "FCM";
        public static readonly string PCM = "PCM";
        public static readonly string PNT = "PNT";
        public static readonly string P3P = "P3P";
        public static readonly string THREEPPNT = "3PPNT";
        public static readonly string FIXCNT = "FIXCNT";

        public static readonly string SYSTEMATIC_SAMPLER_TYPE = "SystematicSelecter";
        public static readonly string BLOCK_SAMPLER_TYPE = "BlockSelecter";
        public static readonly string SRS_SAMPLER_TYPE = "SRSSelecter";
        public static readonly string THREEP_SAMPLER_TYPE = "ThreePSelecter";

        public static readonly string[] RECON_METHODS = { FIX, PNT };
        public static readonly string[] SUPPORTED_METHODS = { H_PCT, STR, S3P, THREEP, FIX, F3P, FCM, PCM, PNT, P3P, THREEPPNT, FIXCNT };
        public static readonly string[] PLOT_METHODS = { FIX, F3P, FCM, PNT, PCM, P3P };
        public static readonly string[] THREE_P_METHODS = { THREEP, S3P, F3P, P3P };
        public static readonly string[] VARIABLE_RADIUS_METHODS = { PCM, PNT, P3P, THREEPPNT };
        public static readonly string[] TALLY_METHODS = { STR, THREEP, S3P, F3P, P3P, PCM, FCM };
        public static readonly string[] MANDITORY_TALLY_METHODS = { STR, THREEP, S3P, PCM, FCM };
        public static readonly string[] UNSUPPORTED_METHODS = { };
    }
}