﻿
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;

namespace BuilderDemo;

[System.CodeDom.Compiler.GeneratedCode("BuilderGenerator", "v1.0.0.0")]
public class PersonBuilder{
    public string Name { get; set; }
    public int Age { get; set; }

    public Person Build() =>
        new Person(Name, Age);
}
