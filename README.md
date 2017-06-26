# elasticsearch-recipes-nest-angular
The application is pretty simple, but extremely flexible in terms of searching data.

![Application](https://lh6.googleusercontent.com/NBUnYg6_wfTDzVqgrab-NNp8EpJsUyEEqINFzBBbSU2n8oiQLoLWe3bVMSfyLtzr3sPo0sSORqd_KPM=w1920-h920-rw)

It supports a number of Elastic query types, among which *[query_string](https://www.elastic.co/guide/en/elasticsearch/reference/current/query-dsl-query-string-query.html), [multi_match](https://www.elastic.co/guide/en/elasticsearch/reference/current/query-dsl-multi-match-query.html), [completion](https://www.elastic.co/guide/en/elasticsearch/reference/current/search-suggesters-completion.html), [more_like_this](https://www.elastic.co/guide/en/elasticsearch/reference/current/query-dsl-mlt-query.html)* and others.

The user inputted string is proccessed in the following way:
> * Exact phrases to look for are marked with ***quotes*** ("***an example phrase***").
> * Each word/phrase that isn't marked in any way (**example**) ***will*** be present in the result set's ingredients.
> * Each word/phrase that is marked with a minus (**-example**) ***will not*** be present in the result set's ingredients.

So, if we want to look for recipes containing ***garlic***, ***tomatoes***, ***chopped onions*** and *not* containing ***eggs*** and ***red-pepper flakes***, we'll have to input the query string:

> **garlic tomatoes -egg* "chopped onions" -"red-pepper flakes"** // Order doesn't matter

The asterisk symbol following the ***egg*** is called a **wildcard** query. It means that it's going to look for words that start with ***egg*** and continue with anything else, *no matter the length* (for example, **egg*** is going to match **eggs, eggplant, eggbeater**, etc.), very similar to SQL's like.

You can substitute the asterisk with a "**?**" sign, which will indicate that Elastic should look for just a **single** missing letter.

This spits out the result:

![Result](https://lh4.googleusercontent.com/qEivmkQ0WMYV3Q9NLBIxrQ8tFCkdLBNRIMMfSXkYUnxf3uWq1RvXX7rF6Ulvk7mZWWvdMmYR7Hd5B84=w1920-h920-rw)
...
