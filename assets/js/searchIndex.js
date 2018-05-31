
var camelCaseTokenizer = function (obj) {
    var previous = '';
    return obj.toString().trim().split(/[\s\-]+|(?=[A-Z])/).reduce(function(acc, cur) {
        var current = cur.toLowerCase();
        if(acc.length === 0) {
            previous = current;
            return acc.concat(current);
        }
        previous = previous.concat(current);
        return acc.concat([current, previous]);
    }, []);
}
lunr.tokenizer.registerFunction(camelCaseTokenizer, 'camelCaseTokenizer')
var searchModule = function() {
    var idMap = [];
    function y(e) { 
        idMap.push(e); 
    }
    var idx = lunr(function() {
        this.field('title', { boost: 10 });
        this.field('content');
        this.field('description', { boost: 5 });
        this.field('tags', { boost: 50 });
        this.ref('id');
        this.tokenizer(camelCaseTokenizer);

        this.pipeline.remove(lunr.stopWordFilter);
        this.pipeline.remove(lunr.stemmer);
    });
    function a(e) { 
        idx.add(e); 
    }

    a({
        id:0,
        title:"IParameterTag",
        content:"IParameterTag",
        description:'',
        tags:''
    });

    a({
        id:1,
        title:"BaseFilters",
        content:"BaseFilters",
        description:'',
        tags:''
    });

    a({
        id:2,
        title:"Parser",
        content:"Parser",
        description:'',
        tags:''
    });

    a({
        id:3,
        title:"ITag",
        content:"ITag",
        description:'',
        tags:''
    });

    a({
        id:4,
        title:"IFilter",
        content:"IFilter",
        description:'',
        tags:''
    });

    a({
        id:5,
        title:"ValidationResult",
        content:"ValidationResult",
        description:'',
        tags:''
    });

    a({
        id:6,
        title:"FilterWithoutArguments",
        content:"FilterWithoutArguments",
        description:'',
        tags:''
    });

    a({
        id:7,
        title:"CompiledString",
        content:"CompiledString",
        description:'',
        tags:''
    });

    a({
        id:8,
        title:"IToken",
        content:"IToken",
        description:'',
        tags:''
    });

    a({
        id:9,
        title:"ParserExtensions",
        content:"ParserExtensions",
        description:'',
        tags:''
    });

    y({
        url:'/Strinken/api/Strinken/IParameterTag',
        title:"IParameterTag",
        description:""
    });

    y({
        url:'/Strinken/api/Strinken/BaseFilters',
        title:"BaseFilters",
        description:""
    });

    y({
        url:'/Strinken/api/Strinken/Parser_1',
        title:"Parser<T>",
        description:""
    });

    y({
        url:'/Strinken/api/Strinken/ITag_1',
        title:"ITag<T>",
        description:""
    });

    y({
        url:'/Strinken/api/Strinken/IFilter',
        title:"IFilter",
        description:""
    });

    y({
        url:'/Strinken/api/Strinken/ValidationResult',
        title:"ValidationResult",
        description:""
    });

    y({
        url:'/Strinken/api/Strinken/FilterWithoutArguments',
        title:"FilterWithoutArguments",
        description:""
    });

    y({
        url:'/Strinken/api/Strinken/CompiledString',
        title:"CompiledString",
        description:""
    });

    y({
        url:'/Strinken/api/Strinken/IToken',
        title:"IToken",
        description:""
    });

    y({
        url:'/Strinken/api/Strinken/ParserExtensions',
        title:"ParserExtensions",
        description:""
    });

    return {
        search: function(q) {
            return idx.search(q).map(function(i) {
                return idMap[i.ref];
            });
        }
    };
}();
