use std::collections::HashMap;
use std::iter::Iterator;

use crate::lexer::token::Token;
use crate::lexer::token_type::TokenType;

pub struct Lexer {
    index: i32,
    text: String,
    punctuators: HashMap<char, &'static TokenType>,
}

impl Lexer {
    pub fn new(original_text: &String) -> Lexer {
        let text = original_text.clone();

        let mut punctuators = HashMap::new();
        for token_type in TokenType::iterator() {
            match token_type.punctuator() {
                Some(c) => punctuators.insert(c, token_type),
                None => None,
            };
        }

        Lexer {
            index: 0,
            text,
            punctuators,
        }
    }
}

impl Iterator for Lexer {
    type Item = Token;

    fn next(&mut self) -> Option<Token> {
        // Once we've reached the end of the string, just return EOF tokens. We'll
        // just keeping returning them as many times as we're asked so that the
        // parser's lookahead doesn't have to worry about running out of tokens.
        let token = Token {
            token_type: TokenType::Eof,
            text: String::from(""),
        };
        Some(token)
    }
}
