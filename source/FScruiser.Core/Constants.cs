﻿using CruiseDAL.DataObjects;
using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser
{
    public class Constants
    {
        public static readonly TreeFieldSetupDO[] DEFAULT_TREE_FIELDS = new TreeFieldSetupDO[]{
            new TreeFieldSetupDO(){
                Field = nameof(Tree.TreeNumber), Heading = "Tree", FieldOrder = 1, ColumnType = "Text" },
            new TreeFieldSetupDO() {
                Field = nameof(Tree.Stratum), Heading = "St", Format = "[Code]" , FieldOrder = 2, ColumnType = "Text"  },
            new TreeFieldSetupDO() {
                Field = nameof(Tree.SampleGroup), Heading = "SG", Format = "[Code]" , FieldOrder = 3, ColumnType = "Text" },
            new TreeFieldSetupDO() {
                Field = nameof(Tree.Species), Heading = "Sp", FieldOrder = 4, ColumnType = "Combo" },
            new TreeFieldSetupDO() {
                Field = nameof(Tree.DBH), Heading = "DBH", FieldOrder = 5, ColumnType = "Text" },
            new TreeFieldSetupDO() {
                Field = nameof(Tree.TotalHeight), Heading = "THT", FieldOrder = 6, ColumnType = "Text" },
            new TreeFieldSetupDO() {
                Field = nameof(Tree.SeenDefectPrimary), Heading = "Def", FieldOrder = 7, ColumnType = "Text" }
        };

        public static readonly LogFieldSetupDO[] DEFAULT_LOG_FIELDS = new LogFieldSetupDO[]{
            new LogFieldSetupDO(){
                Field = nameof(LogDO.LogNumber), Heading = "LogNum", FieldOrder = 1, ColumnType = "Text" },
            new LogFieldSetupDO(){
                Field = nameof(LogDO.Grade), Heading = "Grade", FieldOrder = 2, ColumnType = "Text"},
            new LogFieldSetupDO() {
                Field = nameof(LogDO.SeenDefect), Heading = "PctSeenDef", FieldOrder = 3, ColumnType = "Text"}
        };
    }
}