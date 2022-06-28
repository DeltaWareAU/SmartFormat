# Overview
SmartFormat was built to provide a tool that supported the following requirements.

* Concatenation
* Optional Variables
* Unordered arguments.
* Continued support for standard .Net formatting.

Whilst SmartFormat does have some similarities with .Nets standard way of string formatting it does differ in some significant ways.

## Declaring a Variable
A variable is declared using the following syntax ```{myVariable}```. A declared variable can also be decorated to change how it is formatted.

* **Optional Variable** - Appending a **?** at the end of a variable name informs the parser that if the associated argument is not found for the variable it can be ignored and thus not included in the final output string.
* **Value Formatting** - Appending **:{Format String}** after a variables name describes what kind of formatting should be used for the variable.

If you would like to use a ```{``` or ```}``` in your template without it being treated as key for variable declaration simply double it like ```{{``` or ```}}``` and it will be ignored, the final string output will remove the duplicate.

Now that you've seen how to declare variables let's show some example.


### Example A

FormatString ```"FirstName: {firstName}"```

Arguments ```{ "firstName", "John" }```

Output ```"FirstName: John"```

### Example B

FormatString ```"FirstName: {firstName} LastName: {lastName?}"```

Arguments ```{ "firstName", "John" }```

Output ```"FirstName: John LastName: "```

### Example C

FormatString ```"{date:d}"```

Arguments ```{ "date", new DateTime(2020, 05, 05) }```

Output ```"5/05/2020"```

### Example D

FormatString ```"{date?:d}"```

Arguments ```{ }```

Output ```""```


## Declaring a Concatenated Variable
As you could see from the above examples. It almost acts the same as .Nets string.Format. But now we're going to discuss concatenated variables.

Concatenated variables in conjuctions with nullable variables allows for some really powerful formatting to be done with very little effort.

A concatenated variable is declared using the following syntax ```[{myVariable}]```. A concatenated variable declared like this will act example the same as a standard variable - but we can also decorated it like the following ```[LastName: {lastName}]```. In this example we've added LastName: inside the square brackets to specify the start concatenation, it is also possible to decorate the back like this ```[{seconds} Seconds]```.

If you would like to use a ```[``` or ```]``` in your template without it being treated as key for concatenated variable declaration simply double it like ```[[``` or ```]]``` and it will be ignored, the final string output will remove the duplicate.

Now that you've seen how to declare concatenated variables let's show some example.

### Example A

FormatString ```"[FirstName: {firstName}] [LastName: {lastName?}]"```

Arguments ```{ "firstName", "John" }```

Output ```"FirstName: John"```

### Example B

FormatString ```"[{days?} Days] [{hours?} Hours] [{minutes?} Minutes] [and {seconds?}] Seconds"```

Arguments ```{ "Hours", 5 }, { "Seconds", 30 }```

Output ```"5 Hours and 30 Seconds"```

### Example C

FormatString ```"{baseUrl}/api/{resource}[/{template}][?{query}]"```

Arguments ```{ "baseUrl", "http://localhost" }, { "resource", "Students" }, { "template", 5 }```

Output ```"http://localhost/api/resource/Students/5"```

### Example D

FormatString ```"{baseUrl}/api/{resource}[/{template}][?{query}]"```

Arguments ```{ "baseUrl", "http://localhost" }, { "resource", "Students" }, { "query", "firstName=John&lastName=Smith" }```

Output ```"http://localhost/api/resource/Students?firstName=John&lastName=Smith"```

As you can see from the above examples, some minor adjustments to the syntax of a format template can offer some powerful utility in string formatting.

# Code Example
### Parse from Dictionary Arguments

```csharp
string template = "{baseUrl}/api/{resource}[/{template}][?{query}]";

Dictionary<string, object> args = new Dictionary<string, object>
{
	{ "baseUrl", "http://localhost" }, 
	{ "resource", "Students" }, 
	{ "query", "firstName=John&lastName=Smith" }
};

string text = SmartFormat.Parse(template, args);
```

### Parse from object

```csharp
string template = "{BaseUrl}/api/{Resource}[/{Template}][?{Query}]";

UriTemplate uriTempalte = new UriTemplate
{
	BaseUrl = "http://localhost",
	Resource = "Students",
	Query = "firstName=John&lastName=Smith"
};

string text = SmartFormat.Parse(template, uriTempalte);
```

```csharp
string template = "{BaseUrl}/api/{Resource}[/{Template}][?{Query}]";

string text = SmartFormat.Parse(template, new 
{
	BaseUrl = "http://localhost",
	Resource = "Students",
	Query = "firstName=John&lastName=Smith"
});
```

## Possible Improvements
* Allow a variable to be represeneted by an array, something like repeating the variable for x items.

* Allow simple boolean logic to be perfomed in a Concated Variable. EG ```[{seconds}(>1)seconds:second]```