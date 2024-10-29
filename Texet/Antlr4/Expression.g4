grammar Expression;

expr:   expression EOF;

expression
    : '(' expression ')'             # Parentheses
    | func '(' args ')'              # FunctionCall
    | expression '^' expression      # Power
    | expression ('*' | '/') expression      # MultDiv
    | expression '%' expression      # Modulus
    | expression ('+' | '-') expression      # AddSub
    | expression ('<' | '>' | '=' | '<=' | '>=') expression #Compare
    | NUMBER                         # Number
    ;

func
    : 'max'
    | 'min'
    | 'abs'
    ;

args
    : expression (',' expression)*
    ;

NUMBER  : [0-9]+ ('.' [0-9]+)? ; 
WS      : [ \t\r\n]+ -> skip ;   
