mod lexer;

pub use crate::lexer::lexer::Lexer;
pub use crate::lexer::token::Token;
pub use crate::lexer::token_type::TokenType;

#[cfg(test)]
mod tests {
    use super::{Lexer, Token, TokenType};

    #[test]
    fn get_type() {
        let mut token_type = TokenType::LeftParen;
        let mut result = token_type.punctuator();

        assert_eq!(result, Some('('));

        token_type = TokenType::Eof;
        result = token_type.punctuator();

        assert_eq!(result, None);
    }

    #[test]
    fn lexer() {
        let mut lexer = Lexer::new(&String::from(""));
        let result = lexer.next();
        let token = Token {
            token_type: TokenType::Eof,
            text: String::from(""),
        };

        assert_eq!(result, Some(token));
    }
}
