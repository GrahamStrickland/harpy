use crate::lexer::token_type::TokenType;

#[derive(Debug, PartialEq, Eq)]
pub struct Token {
    token_type: TokenType,
    pub text: String,
}

impl Token {
    pub fn new(token_type: TokenType, text: String) -> Token {
        Self { token_type, text }
    }
}
