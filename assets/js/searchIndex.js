
var camelCaseTokenizer = function (builder) {

  var pipelineFunction = function (token) {
    var previous = '';
    // split camelCaseString to on each word and combined words
    // e.g. camelCaseTokenizer -> ['camel', 'case', 'camelcase', 'tokenizer', 'camelcasetokenizer']
    var tokenStrings = token.toString().trim().split(/[\s\-]+|(?=[A-Z])/).reduce(function(acc, cur) {
      var current = cur.toLowerCase();
      if (acc.length === 0) {
        previous = current;
        return acc.concat(current);
      }
      previous = previous.concat(current);
      return acc.concat([current, previous]);
    }, []);

    // return token for each string
    // will copy any metadata on input token
    return tokenStrings.map(function(tokenString) {
      return token.clone(function(str) {
        return tokenString;
      })
    });
  }

  lunr.Pipeline.registerFunction(pipelineFunction, 'camelCaseTokenizer')

  builder.pipeline.before(lunr.stemmer, pipelineFunction)
}
var searchModule = function() {
    var documents = [];
    var idMap = [];
    function a(a,b) { 
        documents.push(a);
        idMap.push(b); 
    }

    a(
        {
            id:0,
            title:"IToken",
            content:"IToken",
            description:'',
            tags:''
        },
        {
            url:'/Strinken/api/Strinken/IToken',
            title:"IToken",
            description:""
        }
    );
    a(
        {
            id:1,
            title:"FilterWithoutArguments",
            content:"FilterWithoutArguments",
            description:'',
            tags:''
        },
        {
            url:'/Strinken/api/Strinken/FilterWithoutArguments',
            title:"FilterWithoutArguments",
            description:""
        }
    );
    a(
        {
            id:2,
            title:"ValidationResult",
            content:"ValidationResult",
            description:'',
            tags:''
        },
        {
            url:'/Strinken/api/Strinken/ValidationResult',
            title:"ValidationResult",
            description:""
        }
    );
    a(
        {
            id:3,
            title:"Parser",
            content:"Parser",
            description:'',
            tags:''
        },
        {
            url:'/Strinken/api/Strinken/Parser_1',
            title:"Parser<T>",
            description:""
        }
    );
    a(
        {
            id:4,
            title:"ParserExtensions",
            content:"ParserExtensions",
            description:'',
            tags:''
        },
        {
            url:'/Strinken/api/Strinken/ParserExtensions',
            title:"ParserExtensions",
            description:""
        }
    );
    a(
        {
            id:5,
            title:"IFilter",
            content:"IFilter",
            description:'',
            tags:''
        },
        {
            url:'/Strinken/api/Strinken/IFilter',
            title:"IFilter",
            description:""
        }
    );
    a(
        {
            id:6,
            title:"CompiledString",
            content:"CompiledString",
            description:'',
            tags:''
        },
        {
            url:'/Strinken/api/Strinken/CompiledString',
            title:"CompiledString",
            description:""
        }
    );
    a(
        {
            id:7,
            title:"BaseFilters",
            content:"BaseFilters",
            description:'',
            tags:''
        },
        {
            url:'/Strinken/api/Strinken/BaseFilters',
            title:"BaseFilters",
            description:""
        }
    );
    a(
        {
            id:8,
            title:"ITag",
            content:"ITag",
            description:'',
            tags:''
        },
        {
            url:'/Strinken/api/Strinken/ITag_1',
            title:"ITag<T>",
            description:""
        }
    );
    a(
        {
            id:9,
            title:"IParameterTag",
            content:"IParameterTag",
            description:'',
            tags:''
        },
        {
            url:'/Strinken/api/Strinken/IParameterTag',
            title:"IParameterTag",
            description:""
        }
    );
    var idx = lunr(function() {
        this.field('title');
        this.field('content');
        this.field('description');
        this.field('tags');
        this.ref('id');
        this.use(camelCaseTokenizer);

        this.pipeline.remove(lunr.stopWordFilter);
        this.pipeline.remove(lunr.stemmer);
        documents.forEach(function (doc) { this.add(doc) }, this)
    });

    return {
        search: function(q) {
            return idx.search(q).map(function(i) {
                return idMap[i.ref];
            });
        }
    };
}();
