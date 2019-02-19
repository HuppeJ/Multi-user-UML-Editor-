package com.polypaint.polypaint.Model

class ClassForm (id: String, type: Int, formStyle: FormStyle, links: Array<String?>, var className: String, var attributes: Array<Attribute?>, var methods: Array<Method?>):BasicForm(id, type, formStyle, links){}