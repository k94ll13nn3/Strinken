Description: Background on this project.
---

In [Fun With Named Formats, String Parsing, and Edge Cases](http://haacked.com/archive/2009/01/04/fun-with-named-formats-string-parsing-and-edge-cases.aspx/), 
there were some discussions about improving `String.Format` in order to be able to use directly named variable (since C# 6, this is 
now integrated in the language).


String interpolation is great, but it is not easily useable as configuration strings, as the user who will write the string does not know what variables 
can be used and it can lead to using variables that shouldn't be used.


This library is focused on restricting the possible variables to a definite set (tags) that the user can see and only allowing this set. It also give 
some utility functions, like filters that can be applied to the tags, or parameter tags.