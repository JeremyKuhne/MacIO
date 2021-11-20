// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MacIO;

public partial class ProDOSVolume
{
    public enum FileType : byte
    {
        /// <summary>
        ///  Typeless file. (SOS and ProDOS)
        /// </summary>
        Typeless                = 0x00,

        /// <summary>
        ///  Bad block file.
        /// </summary>
        BadBlock                = 0x01,

        /// <summary>
        ///  Pascal code file. (SOS only)
        /// </summary>
        PascalCode              = 0x02,

        /// <summary>
        ///  Pascal text file. (SOS only)
        /// </summary>
        PascalText              = 0x03,

        /// <summary>
        ///  ASCII text file. (SOS and ProDOS)
        /// </summary>
        ASCIIText               = 0x04,

        /// <summary>
        ///  Pascal data file. (SOS only)
        /// </summary>
        PascalData              = 0x05,

        /// <summary>
        ///  General binary file. (SOS and ProDOS)
        /// </summary>
        GeneralBinary           = 0x06,

        /// <summary>
        ///  Font file. (SOS only)
        /// </summary>
        Font                    = 0x07,

        /// <summary>
        ///  Graphics screen file.
        /// </summary>
        GraphicsScreen          = 0x08,

        /// <summary>
        ///  Business BASIC program file. (SOS only)
        /// </summary>
        BusinessBasicProgram    = 0x09,

        /// <summary>
        ///  Business BASIC data file. (SOS only)
        /// </summary>
        BusinessBasicData       = 0x0A,

        /// <summary>
        ///  Word processor file. (SOS only)
        /// </summary>
        WordProcessor           = 0x0B,

        /// <summary>
        ///  SOS system file. (SOS only)
        /// </summary>
        SOSSystem               = 0x0C,

        // * 0x0D, 0x0E SOS reserved

        /// <summary>
        ///  Directory file. (SOS and ProDOS)
        /// </summary>
        Directory               = 0x0F, //

        /// <summary>
        ///  RPS data file. (SOS only)
        /// </summary>
        RPSData                 = 0x10,

        /// <summary>
        ///  RPS index file. (SOS only)
        /// </summary>
        RPSIndex                = 0x11,

        /// <summary>
        ///  AppleFile discard file. (SOS only)
        /// </summary>
        AppleFileDiscard        = 0x12,

        /// <summary>
        ///  AppleFile model file. (SOS only)
        /// </summary>
        AppleFileModel          = 0x13,

        /// <summary>
        ///  AppleFile report format file. (SOS only)
        /// </summary>
        AppleFileReport         = 0x14,

        /// <summary>
        ///  Screen library file. (SOS only)
        /// </summary>
        ScreenLibrary           = 0x15, // * Screen Library file

        // * 0x16 - 0x18 SOS reserved

        /// <summary>
        ///  AppleWorks database file.
        /// </summary>
        AppleWorksDataBase      = 0x19,

        /// <summary>
        ///  AppleWorks word processor file.
        /// </summary>
        AppleWorksWordProcessor = 0x1A,

        /// <summary>
        ///  AppleWorks spreadsheet file.
        /// </summary>
        AppleWorksSpreadsheet   = 0x1B,

        // 0x1C - 0xEE Reserved

        /// <summary>
        ///  Pascal area.
        /// </summary>
        PascalArea              = 0xEF,

        /// <summary>
        ///  ProDOS CI added command file.
        /// </summary>
        ProDOSCommandFile       = 0xF0,

        /// <summary>
        ///  User defined file 1.
        /// </summary>
        UserDefinedFile1        = 0xF1,

        /// <summary>
        ///  User defined file 2.
        /// </summary>
        UserDefinedFile2        = 0xF2,

        /// <summary>
        ///  User defined file 3.
        /// </summary>
        UserDefinedFile3        = 0xF3,

        /// <summary>
        ///  User defined file 4.
        /// </summary>
        UserDefinedFile4        = 0xF4,

        /// <summary>
        ///  User defined file 5.
        /// </summary>
        UserDefinedFile5        = 0xF5,

        /// <summary>
        ///  User defined file 6.
        /// </summary>
        UserDefinedFile6        = 0xF6,

        /// <summary>
        ///  User defined file 7.
        /// </summary>
        UserDefinedFile7        = 0xF7,

        /// <summary>
        ///  User defined file 8.
        /// </summary>
        UserDefinedFile8        = 0xF8,

        // 0xF9 ProDOS reserved

        /// <summary>
        ///  Integer BASIC program file.
        /// </summary>
        IntegerBasicProgram     = 0xFA,

        /// <summary>
        ///  Integer BASIC variables file.
        /// </summary>
        IntegerBasicVariables   = 0xFB,

        /// <summary>
        ///  Applesoft program file.
        /// </summary>
        ApplesoftProgram        = 0xFC,

        /// <summary>
        ///  Applesoft variables file.
        /// </summary>
        ApplesoftVariables      = 0xFD,

        /// <summary>
        ///  Relocatable code file (EDASM).
        /// </summary>
        RelocatableCode         = 0xFE,

        /// <summary>
        ///  ProDOS system file.
        /// </summary>
        ProDOSSystem            = 0xFF
    }
}
