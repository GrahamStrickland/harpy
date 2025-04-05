use crate::lexer::token_type::TokenType;

#[derive(Debug, PartialEq, Eq)]
pub struct Token {
    pub(crate) token_type: TokenType,
    pub(crate) text: String,
}
