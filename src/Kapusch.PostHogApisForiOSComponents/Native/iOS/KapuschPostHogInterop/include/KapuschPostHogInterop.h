#include <stdint.h>
int32_t kph_setup(const char *key, const char *host, int32_t debug, int32_t lifecycle);
int32_t kph_capture(const char *event, const char *properties_json);
int32_t kph_identify(const char *id);
void kph_reset(void); void kph_flush(void); void kph_opt_out(void); void kph_opt_in(void);
char *kph_distinct_id(void); void kph_free(char *value);
