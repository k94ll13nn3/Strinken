
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
        title:"ValidationResult",
        content:"ValidationResult",
        description:'',
        tags:''
    });

    a({
        id:1,
        title:"IFilter",
        content:"IFilter",
        description:'',
        tags:''
    });

    a({
        id:2,
        title:"BaseFilters",
        content:"BaseFilters",
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
        title:"FilterWithoutArguments",
        content:"FilterWithoutArguments",
        description:'',
        tags:''
    });

    a({
        id:5,
        title:"IParameterTag",
        content:"IParameterTag",
        description:'',
        tags:''
    });

    a({
        id:6,
        title:"ParserExtensions",
        content:"ParserExtensions",
        description:'',
        tags:''
    });

    a({
        id:7,
        title:"Parser",
        content:"Parser",
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

    y({
        url:'/Strinken/api/Strinken.Parser/ValidationResult',
        title:"ValidationResult",
        description:""
    });

    y({
        url:'/Strinken/api/Strinken.Parser/IFilter',
        title:"IFilter",
        description:""
    });

    y({
        url:'/Strinken/api/Strinken.Filters/BaseFilters',
        title:"BaseFilters",
        description:""
    });

    y({
        url:'/Strinken/api/Strinken.Parser/ITag_1',
        title:"ITag<T>",
        description:""
    });

    y({
        url:'/Strinken/api/Strinken.Filters/FilterWithoutArguments',
        title:"FilterWithoutArguments",
        description:""
    });

    y({
        url:'/Strinken/api/Strinken.Parser/IParameterTag',
        title:"IParameterTag",
        description:""
    });

    y({
        url:'/Strinken/api/Strinken.Parser/ParserExtensions',
        title:"ParserExtensions",
        description:""
    });

    y({
        url:'/Strinken/api/Strinken.Parser/Parser_1',
        title:"Parser<T>",
        description:""
    });

    y({
        url:'/Strinken/api/Strinken.Parser/IToken',
        title:"IToken",
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
