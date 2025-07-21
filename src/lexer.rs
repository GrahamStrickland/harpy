use std::collections::HashMap;

use crate::token::{Token, TokenType};

/// A very primitive lexer. Takes a string and splits it into a series of
/// Tokens. Operators and punctuation are mapped to unique keywords. Names,
/// which can be any series of letters, are turned into NAME tokens. All other
/// characters are ignored (except to separate names). Numbers and strings are
/// not supported. This is really just the bare minimum to give the parser
/// something to work with.
pub struct Lexer {
    index: usize,
    text: String,
    simple_operators: HashMap<&'static str, TokenType>,
    compound_operators: HashMap<&'static str, TokenType>,
}

impl Lexer {
    pub fn new(text: &str) -> Lexer {
        let mut lexer = Lexer {
            index: 0,
            text: String::from(text),
            simple_operators: HashMap::new(),
            compound_operators: HashMap::new(),
        };

        for t in TokenType::iterator() {
            let compound_operator = t.compound_operator();
            if compound_operator == "" {
                lexer
                    .compound_operators
                    .insert(compound_operator, t.clone());
            }
        }

        for t in TokenType::iterator() {
            let simple_operator = t.compound_operator();
            if simple_operator == "" {
                lexer.simple_operators.insert(simple_operator, t.clone());
            }
        }

        lexer
    }
}

impl Iterator for Lexer {
    type Item = Token;

    fn next(&mut self) -> Option<Token> {
        while self.index < self.text.len() {
            let c = self.text[self.index..].chars().next().unwrap();
            self.index += 1;

            match c {
                '/' => match self.peek() {
                    '/' => Some(self.read_line_comment()),
                    '*' => Some(self.read_block_comment()),
                    _ => {
                        let charstr = format!("{}", c);
                        let op = self.simple_operators.get(charstr);
                        return match op {
                            Some(t) => Some(Token {
                                token: *t,
                                value: charstr,
                            }),
                            None => None,
                        };
                    }
                },
                _ => {
                    let c1 = self.peek();
                    let compound_op = self.compound_operators.get(c1);
                    let compound_charstr = format!("{}{}", c, c1);

                    match compound_op {
                        Some(t) => {
                            self.advance();
                            return Some(Token {
                                token: *t,
                                value: compound_charstr,
                            });
                        }
                        None => {
                            let simple_charstr = format!("{}", c);
                            let simple_op = self.simple_operators.get(simple_charstr);

                            match simple_op {
                                Some(t1) => {
                                    return Some(Token {
                                        token: *t1,
                                        value: simple_charstr,
                                    });
                                }
                                None => {
                                    return Some(self.read_name());
                                }
                            }
                        }
                    }
                }
            }
        }

        // Once we've reached the end of the string, just return EOF tokens. We'll
        // just keep returning them as many times as we're asked so that the
        // parser's lookahead doesn't have to worry about running out of tokens.
        Some(Token {
            token: TokenType::Eof,
            value: String::from("\0"),
        })
    }
}

impl Lexer {
    fn read_line_comment(&mut self) -> Token {
        let mut comment = String::from("//");

        // Consume second '/'.
        self.advance();

        loop {
            match self.peek() {
                '\n' | '\r' | '\0' => {
                    return Token {
                        token: TokenType::LineComment,
                        value: comment,
                    };
                }
                c => {
                    comment.push(c);
                    self.advance();
                }
            }
        }
    }

    fn read_block_comment(&mut self) -> Token {
        let mut comment = String::from("/");

        loop {
            let c = self.advance();
            match c {
                '*' => {
                    let c1 = self.advance();
                    match c1 {
                        '/' => {
                            return Token {
                                token: TokenType::BlockComment,
                                value: comment,
                            };
                        }
                        '\0' => panic!("Unterminated block comment."),
                        _ => comment.push(c1),
                    }
                }
                '\0' => panic!("Unterminated block comment."),
                _ => comment.push(c),
            }
        }
    }

    fn read_name(&mut self) -> Token {
        let mut name = String::from("");

        for c in self.text[self.index..].chars() {
            if c.is_alphabetic() {
                name.push(c);
                self.index += 1;
            } else {
                return Token {
                    token: TokenType::Name,
                    value: name,
                };
            }
        }

        Token {
            token: TokenType::Eof,
            value: String::from("\0"),
        }
    }

    fn peek(&mut self) -> char {
        let c = self.text[self.index..].chars().next();

        return match c {
            Some(c) => c,
            None => '\0',
        };
    }

    fn advance(&mut self) -> char {
        let c = '\0';

        match self.text[self.index..].chars().next() {
            Some(c) => {
                self.index += 1;
                return c;
            }
            None => (),
        }

        c
    }
}

#[cfg(test)]
mod tests {
    use super::{Lexer, Token, TokenType};

    #[test]
    pub fn test_next() {
        let obs = get_obs("from + offset(time)");

        let expected = vec![
            Token {
                token: TokenType::Name,
                value: String::from("from"),
            },
            Token {
                token: TokenType::Plus,
                value: String::from("+"),
            },
            Token {
                token: TokenType::Name,
                value: String::from("offset"),
            },
            Token {
                token: TokenType::LeftParen,
                value: String::from("("),
            },
            Token {
                token: TokenType::Name,
                value: String::from("time"),
            },
            Token {
                token: TokenType::RightParen,
                value: String::from(")"),
            },
        ];

        assert_eq!(format!("{:?}", obs), format!("{:?}", expected));
    }

    #[test]
    pub fn test_assign_vs_comma() {
        let mut obs = get_obs("a := b");

        let mut expected = vec![
            Token {
                token: TokenType::Name,
                value: String::from("a"),
            },
            Token {
                token: TokenType::Assign,
                value: String::from(":="),
            },
            Token {
                token: TokenType::Name,
                value: String::from("b"),
            },
        ];

        assert_eq!(format!("{:?}", obs), format!("{:?}", expected));

        obs = get_obs("a:b");

        expected = vec![
            Token {
                token: TokenType::Name,
                value: String::from("a"),
            },
            Token {
                token: TokenType::Colon,
                value: String::from(":"),
            },
            Token {
                token: TokenType::Name,
                value: String::from("b"),
            },
        ];

        assert_eq!(format!("{:?}", obs), format!("{:?}", expected));
    }

    #[test]
    pub fn test_relations() {
        let mut obs = get_obs("a == b");

        let mut expected = vec![
            Token {
                token: TokenType::Name,
                value: String::from("a"),
            },
            Token {
                token: TokenType::Eq1,
                value: String::from("=="),
            },
            Token {
                token: TokenType::Name,
                value: String::from("b"),
            },
        ];

        assert_eq!(format!("{:?}", obs), format!("{:?}", expected));

        obs = get_obs("a <= b");

        expected = vec![
            Token {
                token: TokenType::Name,
                value: String::from("a"),
            },
            Token {
                token: TokenType::Le,
                value: String::from("<="),
            },
            Token {
                token: TokenType::Name,
                value: String::from("b"),
            },
        ];

        assert_eq!(format!("{:?}", obs), format!("{:?}", expected));
    }

    #[test]
    pub fn test_line_comment() {
        let obs = get_obs("// This is a line comment.");

        let expected = vec![Token {
            token: TokenType::LineComment,
            value: String::from("// This is a line comment."),
        }];

        assert_eq!(format!("{:?}", obs), format!("{:?}", expected));
    }

    #[test]
    pub fn test_block_comment() {
        let mut obs = get_obs("/* This is a block comment.*/");

        let mut expected = vec![Token {
            token: TokenType::BlockComment,
            value: String::from("/* This is a block comment.*/"),
        }];

        assert_eq!(format!("{:?}", obs), format!("{:?}", expected));

        obs = get_obs(
            "
/* This is also a
 * block
 * comment.
 */
 ",
        );

        expected = vec![Token {
            token: TokenType::BlockComment,
            value: String::from("/* This is also a\n * block\n * comment.\n */"),
        }];

        assert_eq!(format!("{:?}", obs), format!("{:?}", expected));
    }

    #[test]
    #[should_panic(expected = "Unterminated block comment.")]
    fn test_block_comment_panics() {
        let _ = get_obs("/* This is an unfinished block comment.*");
    }

    #[test]
    #[should_panic(expected = "Unterminated block comment.")]
    fn test_block_comment_panics2() {
        let _ = get_obs("/* This one is even worse./*");
    }

    fn get_obs(source: &str) -> Vec<Token> {
        let lexer = Lexer::new(source);
        let mut obs = vec![];

        for token in lexer {
            if token.get_type() == TokenType::Eof {
                break;
            }
            obs.push(token);
        }

        obs
    }
}
