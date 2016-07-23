# Grammar

``` html
<parameter> => { <tag> <filter list> }
<tag> => <name>
<filter list> => <filter> <filter list> | epsilon
<filter> => :<name><args>
<args> => +<arg><arg list> | epsilon
<arg list> => ,<arg><arg list> | epsilon
<arg> => <fullName> | <tagname>
<tagname> => =<name>
<name> => [a-zA-Z_\-]*
<fullName> => .*
```