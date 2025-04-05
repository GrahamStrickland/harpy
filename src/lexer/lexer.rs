use std::iter::Iterator;

use crate::lexer::token::Token;
use crate::lexer::token_type::TokenType;

pub struct Lexer {
    index: i32,
    text: String,
}

impl Iterator for Lexer {
    type Item = Token;

    fn next(&mut self) -> Option<Token> {
        let token = Token {
            token_type: TokenType::Eof,
            text: String::from(""),
        };
        Some(token)
    }
}
