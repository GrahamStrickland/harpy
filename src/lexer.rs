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

    fn next(&mut self) -> Option<Self::Item> {
        todo!()
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
                value: "from",
            },
            Token {
                token: TokenType::Plus,
                value: "+",
            },
            Token {
                token: TokenType::Name,
                value: "offset",
            },
            Token {
                token: TokenType::LeftParen,
                value: "(",
            },
            Token {
                token: TokenType::Name,
                value: "time",
            },
            Token {
                token: TokenType::RightParen,
                value: ")",
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
                value: "a",
            },
            Token {
                token: TokenType::Assign,
                value: ":=",
            },
            Token {
                token: TokenType::Name,
                value: "b",
            },
        ];

        assert_eq!(format!("{:?}", obs), format!("{:?}", expected));

        obs = get_obs("a:b");

        expected = vec![
            Token {
                token: TokenType::Name,
                value: "a",
            },
            Token {
                token: TokenType::Colon,
                value: ":",
            },
            Token {
                token: TokenType::Name,
                value: "b",
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
                value: "a",
            },
            Token {
                token: TokenType::Eq1,
                value: "==",
            },
            Token {
                token: TokenType::Name,
                value: "b",
            },
        ];

        assert_eq!(format!("{:?}", obs), format!("{:?}", expected));

        obs = get_obs("a <= b");

        expected = vec![
            Token {
                token: TokenType::Name,
                value: "a",
            },
            Token {
                token: TokenType::Le,
                value: "<=",
            },
            Token {
                token: TokenType::Name,
                value: "b",
            },
        ];

        assert_eq!(format!("{:?}", obs), format!("{:?}", expected));
    }

    #[test]
    pub fn test_line_comment() {
        let obs = get_obs("// This is a line comment.");

        let expected = vec![Token {
            token: TokenType::LineComment,
            value: "// This is a line comment.",
        }];

        assert_eq!(format!("{:?}", obs), format!("{:?}", expected));
    }

    #[test]
    pub fn test_block_comment() {
        let mut obs = get_obs("/* This is a block comment.*/");

        let mut expected = vec![Token {
            token: TokenType::BlockComment,
            value: "/* This is a block comment.*/",
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
            value: "/* This is also a\n * block\n * comment.\n */",
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

        for token in lexer.iter() {
            if token.get_type() == TokenType::Eof {
                break;
            }
            obs.append(token);
        }

        obs
    }
}
