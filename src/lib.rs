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
        let mut lexer = Lexer::new("");
        let result = lexer.next();
        let token = Token {
            token_type: TokenType::Eof,
            text: String::from(""),
        };

        assert_eq!(result, Some(token));

        let mut lexer = Lexer::new("Hello");
        let result = lexer.next();
        let token = Token {
            token_type: TokenType::Name,
            text: String::from("Hello"),
        };

        assert_eq!(result, Some(token));

        let mut lexer = Lexer::new("from + offset(time)");
        // TODO: Put this into some kind of collection, it's not exactly DRY...
        assert_eq!(
            lexer.next(),
            Some(Token {
                token_type: TokenType::Name,
                text: String::from("from"),
            })
        );
        assert_eq!(
            lexer.next(),
            Some(Token {
                token_type: TokenType::Plus,
                text: String::from("+"),
            })
        );
        assert_eq!(
            lexer.next(),
            Some(Token {
                token_type: TokenType::Name,
                text: String::from("offset"),
            })
        );
        assert_eq!(
            lexer.next(),
            Some(Token {
                token_type: TokenType::LeftParen,
                text: String::from("("),
            })
        );
        assert_eq!(
            lexer.next(),
            Some(Token {
                token_type: TokenType::Name,
                text: String::from("time"),
            })
        );
        assert_eq!(
            lexer.next(),
            Some(Token {
                token_type: TokenType::RightParen,
                text: String::from(")"),
            })
        );
        assert_eq!(
            lexer.next(),
            Some(Token {
                token_type: TokenType::Eof,
                text: String::from(""),
            })
        );
    }
}
