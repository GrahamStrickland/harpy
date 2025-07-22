use std::collections::{HashMap, VecDeque};

use crate::lexer::Lexer;
use crate::token::{Token, TokenType};

pub struct Parser {
    tokens: Iterator<Token>, 
    read: VecDecque<Token>,
    infix_parselets: HashMap<TokenType, InfixParselet>,
    prefix_parselets: HashMap<TokenType, PrefixParselet>,
}

impl Parser {
    pub fn new(tokens: Iterator<Token>) {
        Parser {
            tokens: tokens,
            read: VecDeque::new(),
            infix_parselets: HashMap::new(),
            prefix_parselets: HashMap::new(),
        }
    }

    pub fn register(&mut self, token: TokenType, parselet: Parselet) {
        match parselet {
            PrefixParselet => self.prefix_parselets.insert(token, parselet),
            InfixParselet => self.infix_parselets.insert(token, parselet),
        }
    }

    pub fn parse_expression(&mut self, precedence: usize) {
        let mut token = self.consume(); 
        let prefix = self.prefix_parselets.get(&token.token_type);

        match prefix {
            Some => {
                let mut left = prefix.parse(self, token);
                
                while precedence < self.get_precedence() {
                    token = self.consume();

                    let infix = self.infix_parselets.get(&token.token_type);
                    left = infix.parse(self, left, token); 
                }

                return left;
            },
            None => panic!(format!("Could not parse '{}'", token.value)),
        }
    }

    pub fn match(&mut self, expected: TokenType) -> bool {
        let token = self.look_ahead(0);

        if token.type != expected {
            return false;
        }

        self.consume();
        
        true
    }

    pub fn consume(&mut self, expected: Option<TokenType>) -> Token {
        // Make sure we've read the token.
        token = self.look_ahead(0);

        match expected {
            Some(t) => {
                if t.type != expected {
                    panic!(format!("Expected token {} and found {}", expected, token.type))
                }

                return self.read.pop_front(0);
            },
            None => panic!(format!("Expected token {} and found {}", expected, token.type)),
        }
    }

    fn look_ahead(&mut self, distance: usize) {
        // Read in as many as needed.
        while distance >= self.read.len() {
            self.read.push_back(self._tokens.next());
        }

        self.read.get(distance)
    }

    fn get_precedence(&mut self) -> usize {
        let type = self.look_ahead(0).type;
        let parser = self.infix_parselets.get(type);

        match parser {
            Some(p) => p.get_precedence(),
            None => 0,
        }
    }
}

#[cfg(test)]
mod tests {
    use super::{Parser, Token, TokenType};
}
