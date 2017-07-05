# elasticsearch-recipes-nest-angular
The application is pretty simple, but extremely flexible in terms of searching data.

![Application][app_full]

Behind the scenes, the user inputted query is converted to a [query_string][qsdocs] query that is sent to Elastic. The [query_string][qsdocs] query provides native support for a number of [search syntax][qssearchsyntax] options, but we're mostly interested in [wildcards][qswildcards], [boolean operators][qsbooloperators], [phrase matching][qsphrasematching] and [boosting][qsboosting]. 

The user inputted string is proccessed in the following way:
> * Exact phrases to look for are marked with ***quotes*** ("***an example phrase***").
> * Each word/phrase that isn't marked in any way (**example**) ***will*** be present in the result set's ingredients.
> * Each word/phrase that is marked with a minus (**-example**) ***will not*** be present in the result set's ingredients.
> * Each word/phrase can be [boosted][qsboosting] to become more relevant (score higher) in the search results.

So, if we want to look for recipes containing ***garlic***, ***tomatoes***, ***chopped onions*** and *not* containing ***eggs*** and ***red-pepper flakes***, we'll have to input the query string:

> **garlic^2 tomatoes -egg* "chopped onions" -"red-pepper flakes"** // Order doesn't matter

As you can see, we've boosted the garlic. This means that recipes where garlic appears more frequently will score higher.

The asterisk symbol following the ***egg*** is called a **wildcard** query. It means that it's going to look for words that start with ***egg*** and continue with anything else, *no matter the length* (for example, **egg*** is going to match **eggs, eggplant, eggbeater**, etc.), very similar to SQL's like.

You can substitute the asterisk with a "**?**" sign, which will indicate that Elastic should look for just a **single** missing letter.

This spits out the result: 
![Result][sample_query_result]
...

[app_full]: http://i68.tinypic.com/230e2e.png
[sample_query_result]: http://i66.tinypic.com/2s7xw7l.png

[qsdocs]: https://www.elastic.co/guide/en/elasticsearch/reference/current/query-dsl-query-string-query.html
[qssearchsyntax]: https://www.elastic.co/guide/en/elasticsearch/reference/current/query-dsl-query-string-query.html#query-string-syntax
[qsbooloperators]: https://www.elastic.co/guide/en/elasticsearch/reference/current/query-dsl-query-string-query.html#_boolean_operators
[qsboosting]: https://www.elastic.co/guide/en/elasticsearch/reference/current/query-dsl-query-string-query.html#_boosting
[qswildcards]: https://www.elastic.co/guide/en/elasticsearch/reference/current/query-dsl-query-string-query.html#_wildcards
[qsphrasematching]: https://www.elastic.co/guide/en/elasticsearch/reference/current/query-dsl-query-string-query.html#query-string-syntax
