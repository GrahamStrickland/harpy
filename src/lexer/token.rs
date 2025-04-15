use crate::lexer::token_type::TokenType;

#[derive(Debug, Clone, PartialEq, Eq)]
pub struct Token {
    pub token_type: TokenType,
    pub text: String,
}

impl Token {
    pub fn new(token_type: TokenType, text: String) -> Token {
        Self { token_type, text }
    }
}
