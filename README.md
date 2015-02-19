[![Build status](https://ci.appveyor.com/api/projects/status/42ywstwoahcfr6o9?svg=true)](https://ci.appveyor.com/project/buchmoyerm/lib-ph4n)

ph4n
===============

Collection of utilities written in .net

ph4n is mostly documented with xml style comments and annotations. Example usages can also be found in the test project.


Common
---------------

Contains basic helper and utility classes. Common also includes the jetbrains annotations.
Common can simply throw an exception on invalid arguments from `ph4n.Common.Validate`

    public void MyFunc(object funcArg) {
        Validate.ArgumentNotNull(funcArg, "funcArg"); //does check and throws exception if invalid
    }

Most of the functionality of Common (such as Swap, UpdateVal, IsZeroOrNull) is pretty self explanatory.

    int first = 1;
    int second = 2;
    GenericUtil.Swap(ref first, ref second) //swaps the value of first and second
    
    bool valueUpdated = GenericUtil.UpdateVal(ref second, 6); //updates the value and returns if the value is new (in this case true)
    valueUpdated = GenericUtil.UpdateVal(ref second, 6); //valuedUpated = false
    
ph4n can be used for easy type conversions. Especially in instances where `Convert.ChangeType` wont work, like converting a string to an enum

    var familyStr = "InterNetwork";
    AddressFamily familyEnum = familyStr.ConvertTo<AddressFamily>();

`SafeSubstring` will return a substring without throwing an exception from invalid index/length parameters

    string myStr = "small string";
    myStr.SafeSubstring(0,200); //returns "small string"
    myStr.SafeSubstring(6,100); //returns "string"
    myStr.SafeSubstring(100,200); returns string.empty
