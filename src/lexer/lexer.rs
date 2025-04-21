use std::collections::HashMap;
use std::iter::{Iterator, Peekable, from_fn, once};
use std::str::Chars;

use crate::lexer::token::Token;
use crate::lexer::token_type::TokenType;

pub struct Lexer<'a> {
    chars: Peekable<Chars<'a>>,
    punctuators: HashMap<char, &'static TokenType>,
}

impl<'a> Lexer<'a> {
    pub fn new(text: &'a str) -> Self {
        let mut punctuators = HashMap::new();
        for token_type in TokenType::iterator() {
            match token_type.punctuator() {
                Some(c) => punctuators.insert(c, token_type),
                None => None,
            };
        }

        Lexer {
            chars: text.chars().peekable(),
            punctuators,
        }
    }
}

impl<'a> Iterator for Lexer<'a> {
    type Item = Token;

    fn next(&mut self) -> Option<Token> {
        while let Some(c) = self.chars.next() {
            if self.punctuators.contains_key(&c) {
                return Some(Token {
                    token_type: (*(*self.punctuators.get(&c).unwrap())).clone(),
                    text: String::from(c),
                });
            } else if c.is_alphabetic() {
                let name = once(c)
                    .chain(from_fn(|| {
                        self.chars.by_ref().next_if(|s| s.is_alphabetic())
                    }))
                    .collect::<String>()
                    .parse()
                    .unwrap();

                return Some(Token {
                    token_type: TokenType::Name,
                    text: name,
                });
            } // else ignore all other characters (whitespace, etc.)
        }

        // Once we've reached the end of the string, just return EOF tokens. We'll
        // just keeping returning them as many times as we're asked so that the
        // parser's lookahead doesn't have to worry about running out of tokens.
        Some(Token {
            token_type: TokenType::Eof,
            text: String::from(""),
        })
    }
}
