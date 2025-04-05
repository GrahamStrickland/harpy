use crate::lexer::token_type::TokenType;

pub struct Token {
    pub(crate) token_type: TokenType,
    pub(crate) text: String,
}
