# Setl
Simple ETL.

A stripped down, modified, modernized version of 
[RhinoETL](https://github.com/ayende/rhino-etl) with utilities focused on batch processing of
*legacy* data.

## Overview





### TextSerializer
A `TextSerializer` intance can be used to either *deserialize* or *serialize* 
a line in a fixed-width text file.

#### Features
* A single `TextSerializer` instance can be reused to deserialize to multiple outputs.
* The serializer can be used to deserialize a single line and return the 
  deserialized object.
* The serializer can be used to serialize a single object and return the 
  serialized string.
* The serializer has a fluent API which makes it easy to configure the 
  serializer at runtime.

The current `TextSerializer` implementation only deals with parsing fixed-width 
files.
